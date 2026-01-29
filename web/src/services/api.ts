import { WeekendsLeftRequest, WeekendsLeftResponse, ApiError } from '../types/api';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://dev-myweekendsleft-api.azurewebsites.net';

export class ApiException extends Error {
  constructor(
    public status: number,
    public title: string,
    public detail: string
  ) {
    super(detail);
    this.name = 'ApiException';
  }
}

export async function getWeekendsLeft(request: WeekendsLeftRequest): Promise<WeekendsLeftResponse> {
  const params = new URLSearchParams({
    age: request.age.toString(),
    gender: request.gender,
    country: request.country,
  });

  const response = await fetch(`${API_BASE_URL}/api/getweekends?${params}`, {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
    },
  });

  if (!response.ok) {
    const errorData: ApiError = await response.json().catch(() => ({
      title: 'Error',
      detail: 'An unexpected error occurred',
      status: response.status,
    }));
    throw new ApiException(response.status, errorData.title, errorData.detail);
  }

  return response.json();
}

export async function getVersion(): Promise<{ buildNumber: string; environment: string }> {
  const response = await fetch(`${API_BASE_URL}/api/version`, {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
    },
  });

  if (!response.ok) {
    throw new ApiException(response.status, 'Error', 'Failed to fetch version');
  }

  return response.json();
}
