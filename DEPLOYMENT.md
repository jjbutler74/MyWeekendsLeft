# 🚀 Deployment Quick Start Guide

This guide will help you deploy the MyWeekendsLeft API to Azure using GitHub Actions.

## Prerequisites

- ✅ Azure subscription
- ✅ GitHub repository (this repo)
- ✅ Azure CLI installed (`az` command)
- ✅ Terraform installed (>= 1.0)

---

## Step 1: Deploy Azure Infrastructure

### Option A: Using Terraform (Recommended)

```bash
# Navigate to Terraform directory
cd infrastructure/tf/prod

# Initialize Terraform
terraform init

# Preview what will be created
terraform plan -var="environment=prod"

# Create the infrastructure
terraform apply -var="environment=prod"

# Save the outputs
terraform output
```

**Important**: Save the `app_service_name` from the output - you'll need it for GitHub Actions.

### Option B: Manual Azure Portal Setup

If you prefer not to use Terraform, create these resources manually:
1. Resource Group: `rg-mwl-prod-001`
2. App Service Plan: `plan-mwl-prod-001` (Windows, S1)
3. App Service: `app-mwl-prod-001` (.NET 9)
4. Application Insights: `appi-mwl-prod-001`

---

## Step 2: Configure GitHub Secrets

### Get Publish Profiles from Azure

```bash
# Get production publish profile
az webapp deployment list-publishing-profiles \
  --name app-mwl-prod-001 \
  --resource-group rg-mwl-prod-001 \
  --xml

# Copy the entire XML output
```

### Add Secret to GitHub

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add this secret:

| Secret Name | Value |
|------------|-------|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Paste the XML output from above |

---

## Step 3: Trigger First Deployment

### Method 1: Push to Main (Automatic)

```bash
git add .
git commit -m "Configure deployment"
git push origin main
```

The GitHub Actions workflow will automatically:
1. ✅ Build the application
2. ✅ Run all 39 tests
3. ✅ Deploy to production

### Method 2: Manual Trigger

1. Go to **Actions** tab in GitHub
2. Select "Build, Test, and Deploy"
3. Click **Run workflow**
4. Select `main` branch
5. Click **Run workflow**

---

## Step 4: Verify Deployment

### Check GitHub Actions

1. Go to **Actions** tab
2. Watch the workflow progress
3. All steps should show green ✅

### Test the Deployed API

```bash
# Test health endpoint (replace with your URL)
curl https://app-mwl-prod-001.azurewebsites.net/health

# Test API endpoint
curl "https://app-mwl-prod-001.azurewebsites.net/api/getweekends/?age=45&gender=male&country=USA"

```

Expected responses:
- Health: `Healthy`
- API: JSON with weekend calculations

---

## Step 5: (Optional) Configure Environment Protection

For production approvals:

1. Go to **Settings** → **Environments**
2. Click **New environment**
3. Name it `Production`
4. Check **Required reviewers**
5. Add yourself as a reviewer
6. Save

Now deployments to production will require manual approval!

---

## 🎉 You're Done!

Your API is now deployed with:

- ✅ Automated CI/CD pipeline
- ✅ Direct to production deployments
- ✅ Health monitoring
- ✅ Application Insights telemetry
- ✅ All 39 tests running on every deployment

---

## 📊 What's Next?

### Monitor Your API

**Application Insights URL:**
```
https://portal.azure.com/#resource/subscriptions/{subscription-id}/resourceGroups/rg-mwl-prod-001/providers/microsoft.insights/components/appi-mwl-prod-001
```

**Key Metrics:**
- Request rate and response times
- Failed requests
- Dependencies (population.io API calls)
- Exceptions

### View Logs

```bash
# Stream live logs
az webapp log tail --name app-mwl-prod-001 --resource-group rg-mwl-prod-001

# Or view in Azure Portal:
# App Service → Monitoring → Log stream
```

### Make Changes

1. Create a branch: `git checkout -b feature/my-feature`
2. Make changes and commit
3. Push and create PR
4. GitHub Actions runs tests automatically
5. Merge to `main` when ready
6. Automatic deployment triggers!

---

## 🔧 Troubleshooting

### Deployment Fails with "Unauthorized"

**Solution**: Regenerate publish profiles and update GitHub secrets

```bash
# Get new profiles
az webapp deployment list-publishing-profiles --name app-mwl-prod-001 --resource-group rg-mwl-prod-001 --xml

# Update GitHub secret AZURE_WEBAPP_PUBLISH_PROFILE with new XML
```

### Tests Fail in Pipeline

**Solution**: Run tests locally first

```bash
cd api/MWL
dotnet test --configuration Release

# Fix any failing tests before pushing
```

### Health Check Fails

**Solution**: Check app logs in Azure Portal

```bash
az webapp log tail --name app-mwl-prod-001 --resource-group rg-mwl-prod-001

# Look for startup errors or exceptions
```

### App is Slow/Timing Out

**Solution**: Check Application Insights for slow dependencies

The population.io API might be slow - check the retry policies and caching.

---

## 📞 Need Help?

- **Documentation**: `infrastructure/README.md`
- **GitHub Actions Logs**: Actions tab in GitHub
- **Azure Logs**: Azure Portal → App Service → Monitoring
- **Application Insights**: Azure Portal → Application Insights

---

## 🔄 Updating Infrastructure

If you need to change infrastructure (e.g., scale up):

```bash
cd infrastructure/tf/prod

# Edit main.tf (e.g., change sku_name to "P1V2")
vim main.tf

# Preview changes
terraform plan -var="environment=prod"

# Apply if looks good
terraform apply -var="environment=prod"
```

---

**Happy Deploying! 🎉**
