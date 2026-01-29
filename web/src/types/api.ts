export interface WeekendsLeftRequest {
  age: number;
  gender: 'Male' | 'Female';
  country: string;
}

export interface WeekendsLeftResponse {
  estimatedWeekendsLeft: number;
  estimatedAgeOfDeath: number;
  estimatedDayOfDeath: string;
  message: string;
  errors: string[] | null;
}

export interface VersionInfo {
  buildNumber: string;
  environment: string;
}

export interface ApiError {
  title: string;
  detail: string;
  status: number;
}

export interface Country {
  code: string;
  name: string;
  flag: string;
}

export const COUNTRIES: Country[] = [
  { code: 'USA', name: 'United States', flag: '🇺🇸' },
  { code: 'GBR', name: 'United Kingdom', flag: '🇬🇧' },
  { code: 'CAN', name: 'Canada', flag: '🇨🇦' },
  { code: 'AUS', name: 'Australia', flag: '🇦🇺' },
  { code: 'NZL', name: 'New Zealand', flag: '🇳🇿' },
  { code: 'DEU', name: 'Germany', flag: '🇩🇪' },
  { code: 'FRA', name: 'France', flag: '🇫🇷' },
  { code: 'JPN', name: 'Japan', flag: '🇯🇵' },
  { code: 'ITA', name: 'Italy', flag: '🇮🇹' },
  { code: 'ESP', name: 'Spain', flag: '🇪🇸' },
  { code: 'NLD', name: 'Netherlands', flag: '🇳🇱' },
  { code: 'BRA', name: 'Brazil', flag: '🇧🇷' },
  { code: 'MEX', name: 'Mexico', flag: '🇲🇽' },
  { code: 'IND', name: 'India', flag: '🇮🇳' },
  { code: 'CHN', name: 'China', flag: '🇨🇳' },
];
