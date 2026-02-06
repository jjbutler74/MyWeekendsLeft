import { useState } from 'react';
import { WeekendsLeftRequest, COUNTRIES } from '../types/api';

interface CalculatorProps {
  onCalculate: (request: WeekendsLeftRequest) => void;
  isLoading: boolean;
}

export function Calculator({ onCalculate, isLoading }: CalculatorProps) {
  const [age, setAge] = useState<number>(35);
  const [gender, setGender] = useState<'Male' | 'Female'>('Male');
  const [country, setCountry] = useState<string>('USA');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onCalculate({ age, gender, country });
  };

  return (
    <form onSubmit={handleSubmit} className="w-full max-w-xl mx-auto">
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-8 space-y-6">
        <h2 className="text-2xl font-semibold text-charcoal dark:text-gray-100 text-center mb-6">
          How many weekends do you have left?
        </h2>

        {/* Age Input */}
        <div className="space-y-2">
          <label htmlFor="age" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Your Age
          </label>
          <input
            type="number"
            id="age"
            min={1}
            max={120}
            value={age}
            onChange={(e) => setAge(Math.max(1, Math.min(120, parseInt(e.target.value) || 1)))}
            className="w-full px-4 py-3 text-lg border-2 border-gray-200 dark:border-gray-600 rounded-xl focus:border-sunset-500 focus:ring-0 transition-colors text-center font-semibold bg-white dark:bg-gray-700 text-charcoal dark:text-gray-100"
          />
          <input
            type="range"
            min={1}
            max={100}
            value={age}
            onChange={(e) => setAge(parseInt(e.target.value))}
            className="w-full h-2 bg-gray-200 dark:bg-gray-600 rounded-lg appearance-none cursor-pointer accent-sunset-500"
          />
        </div>

        {/* Gender Selection */}
        <div className="space-y-2">
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Gender
          </label>
          <div className="grid grid-cols-2 gap-4">
            <button
              type="button"
              onClick={() => setGender('Male')}
              className={`px-6 py-3 rounded-xl font-medium transition-all ${
                gender === 'Male'
                  ? 'bg-ocean-400 text-white shadow-md'
                  : 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
              }`}
            >
              Male
            </button>
            <button
              type="button"
              onClick={() => setGender('Female')}
              className={`px-6 py-3 rounded-xl font-medium transition-all ${
                gender === 'Female'
                  ? 'bg-ocean-400 text-white shadow-md'
                  : 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
              }`}
            >
              Female
            </button>
          </div>
        </div>

        {/* Country Selection */}
        <div className="space-y-2">
          <label htmlFor="country" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
            Country
          </label>
          <select
            id="country"
            value={country}
            onChange={(e) => setCountry(e.target.value)}
            className="w-full px-4 py-3 text-lg border-2 border-gray-200 dark:border-gray-600 rounded-xl focus:border-sunset-500 focus:ring-0 transition-colors bg-white dark:bg-gray-700 text-charcoal dark:text-gray-100"
          >
            {COUNTRIES.map((c) => (
              <option key={c.code} value={c.code}>
                {c.flag} {c.name}
              </option>
            ))}
          </select>
        </div>

        {/* Submit Button */}
        <button
          type="submit"
          disabled={isLoading}
          className="w-full py-4 px-6 bg-sunset-500 hover:bg-sunset-600 disabled:bg-sunset-300 text-white font-semibold text-lg rounded-xl shadow-lg hover:shadow-xl transition-all transform hover:scale-[1.02] disabled:scale-100 disabled:cursor-not-allowed"
        >
          {isLoading ? (
            <span className="flex items-center justify-center gap-2">
              <svg className="animate-spin h-5 w-5" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
              </svg>
              Calculating...
            </span>
          ) : (
            'Calculate My Weekends'
          )}
        </button>
      </div>
    </form>
  );
}
