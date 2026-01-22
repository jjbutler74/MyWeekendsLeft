# 🚀 Deployment Quick Start Guide

This guide will help you deploy the MyWeekendsLeft API to Azure using GitHub Actions.

## Prerequisites

- ✅ Azure subscription with existing App Services
- ✅ GitHub repository (this repo)
- ✅ Azure CLI installed (`az` command) - optional for Terraform

---

## Existing Azure Resources

This project uses **existing** Azure resources:

### DEV Environment
- Resource Group: `dev`
- App Service: `dev-myweekendsleft-api`
- URL: https://dev-myweekendsleft-api.azurewebsites.net

### UAT Environment
- Resource Group: `uat`
- App Service: `uat-myweekendsleft-api`
- URL: https://uat-myweekendsleft-api.azurewebsites.net

---

## Step 1: Configure GitHub Secrets

You need to add publish profiles for both environments.

### Get Publish Profiles from Azure

```bash
# DEV publish profile
az webapp deployment list-publishing-profiles \
  --name dev-myweekendsleft-api \
  --resource-group dev \
  --xml

# Copy the entire XML output
```

```bash
# UAT publish profile
az webapp deployment list-publishing-profiles \
  --name uat-myweekendsleft-api \
  --resource-group uat \
  --xml

# Copy the entire XML output
```

### Add Secrets to GitHub

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add these two secrets:

| Secret Name | Value | Environment |
|------------|-------|-------------|
| `AZURE_WEBAPP_PUBLISH_PROFILE_DEV` | Paste the **DEV** XML | DEV |
| `AZURE_WEBAPP_PUBLISH_PROFILE_UAT` | Paste the **UAT** XML | UAT |

---

## Step 2: Understand the Deployment Workflow

### Automatic Deployments

**DEV Environment:**
- Trigger: Push to `develop` branch
- Workflow: Build → Test → Deploy to DEV

**UAT Environment:**
- Trigger: Push to `main` branch
- Workflow: Build → Test → Deploy to UAT

### Manual Deployments

You can manually trigger deployments to either environment:

1. Go to **Actions** tab in GitHub
2. Select "Build, Test, and Deploy"
3. Click **Run workflow**
4. Select branch (main or develop)
5. Choose environment (dev or uat)
6. Click **Run workflow**

---

## Step 3: Deploy to DEV

### Option A: Push to develop branch

```bash
git checkout -b develop
git add .
git commit -m "Configure deployment"
git push origin develop
```

The GitHub Actions workflow will automatically:
1. ✅ Build the application
2. ✅ Run all 39 tests
3. ✅ Deploy to DEV environment

### Option B: Manual trigger

See "Manual Deployments" above

---

## Step 4: Deploy to UAT

### Option A: Merge to main branch

```bash
git checkout main
git merge develop
git push origin main
```

The GitHub Actions workflow will automatically:
1. ✅ Build the application
2. ✅ Run all 39 tests
3. ✅ Deploy to UAT environment

### Option B: Manual trigger

See "Manual Deployments" above

---

## Step 5: Verify Deployment

### Check GitHub Actions

1. Go to **Actions** tab
2. Watch the workflow progress
3. All steps should show green ✅

### Test the Deployed APIs

**DEV:**
```bash
# Health check
curl https://dev-myweekendsleft-api.azurewebsites.net/health

# API endpoint
curl "https://dev-myweekendsleft-api.azurewebsites.net/api/getweekends/?age=45&gender=male&country=USA"
```

**UAT:**
```bash
# Health check
curl https://uat-myweekendsleft-api.azurewebsites.net/health

# API endpoint
curl "https://uat-myweekendsleft-api.azurewebsites.net/api/getweekends/?age=45&gender=male&country=USA"
```

Expected responses:
- Health: `Healthy`
- API: JSON with weekend calculations

---

## Step 6: (Optional) Configure Terraform

If you want to manage your infrastructure with Terraform:

### For UAT:
```bash
cd infrastructure/tf/uat

# Initialize Terraform
terraform init

# Import existing resources
terraform import azurerm_windows_web_app.main /subscriptions/{subscription-id}/resourceGroups/uat/providers/Microsoft.Web/sites/uat-myweekendsleft-api

# Plan changes
terraform plan

# Apply if needed
terraform apply
```

### For DEV:
```bash
cd infrastructure/tf/dev

# Initialize Terraform
terraform init

# Import existing resources
terraform import azurerm_windows_web_app.main /subscriptions/{subscription-id}/resourceGroups/dev/providers/Microsoft.Web/sites/dev-myweekendsleft-api

# Plan changes
terraform plan

# Apply if needed
terraform apply
```

---

## 🎉 You're Done!

Your API is now deployed with:

- ✅ Automated CI/CD pipeline for both DEV and UAT
- ✅ Separate branches for environment control
- ✅ Health monitoring on both environments
- ✅ Application Insights telemetry
- ✅ All 39 tests running on every deployment

---

## 📊 Deployment Flow

```
┌─────────────┐
│   develop   │  Push → Build & Test → Deploy to DEV
└─────────────┘

┌─────────────┐
│     main    │  Push → Build & Test → Deploy to UAT
└─────────────┘
```

---

## 🔧 Troubleshooting

### ❌ "Deployment failed: Unauthorized"

**Solution**: Regenerate publish profiles and update GitHub secrets

```bash
# Get new profile for DEV
az webapp deployment list-publishing-profiles --name dev-myweekendsleft-api --resource-group dev --xml

# Update GitHub secret AZURE_WEBAPP_PUBLISH_PROFILE_DEV with new XML
```

### ❌ "Tests failed"

**Solution**: Run tests locally first

```bash
cd api/MWL
dotnet test --configuration Release

# Fix any failing tests before pushing
```

### ❌ "Health check failed"

**Solution**: Check app logs in Azure Portal

1. Go to Azure Portal
2. Navigate to your App Service (dev or uat)
3. Go to Monitoring → Log stream
4. Look for startup errors or exceptions

---

## 📞 Need Help?

- **Documentation**: `infrastructure/README.md`
- **GitHub Actions Logs**: Actions tab in GitHub
- **Azure Logs**: Azure Portal → App Service → Monitoring
- **Application Insights**: Azure Portal → Application Insights

---

**Happy Deploying! 🎉**
