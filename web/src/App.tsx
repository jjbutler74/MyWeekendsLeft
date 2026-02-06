import { useState, useEffect } from 'react';
import { Calculator } from './components/Calculator';
import { Results } from './components/Results';
import { ResultsSkeleton } from './components/ResultsSkeleton';
import { DarkModeToggle } from './components/DarkModeToggle';
import { useDarkMode } from './hooks/useDarkMode';
import { getWeekendsLeft, getVersion, ApiException } from './services/api';
import { WeekendsLeftRequest, WeekendsLeftResponse } from './types/api';

function App() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [result, setResult] = useState<WeekendsLeftResponse | null>(null);
  const [lastRequest, setLastRequest] = useState<WeekendsLeftRequest | null>(null);
  const [apiVersion, setApiVersion] = useState<string | null>(null);
  const { isDark, toggle: toggleDark } = useDarkMode();

  // Fetch API version on mount
  useEffect(() => {
    getVersion()
      .then((v) => setApiVersion(v.build))
      .catch(() => setApiVersion(null));
  }, []);

  const handleCalculate = async (request: WeekendsLeftRequest) => {
    setIsLoading(true);
    setError(null);

    try {
      const data = await getWeekendsLeft(request);
      setResult(data);
      setLastRequest(request);
    } catch (err) {
      if (err instanceof ApiException) {
        setError(err.detail);
      } else {
        setError('Unable to connect to the server. Please try again later.');
      }
      setResult(null);
    } finally {
      setIsLoading(false);
    }
  };

  const handleReset = () => {
    setResult(null);
    setError(null);
  };

  return (
    <div className="min-h-screen bg-cream dark:bg-gray-900 transition-colors">
      {/* Header */}
      <header className="py-6 px-4">
        <div className="max-w-4xl mx-auto flex items-center justify-between">
          <div className="flex items-center gap-3">
            <span className="text-3xl">🌅</span>
            <h1 className="text-2xl font-bold text-charcoal dark:text-gray-100">MyWeekendsLeft</h1>
          </div>
          <div className="flex items-center gap-3">
            <DarkModeToggle isDark={isDark} onToggle={toggleDark} />
            <a
              href="https://github.com/jjbutler74/MyWeekendsLeft"
              target="_blank"
              rel="noopener noreferrer"
              className="text-gray-500 hover:text-charcoal dark:text-gray-400 dark:hover:text-gray-100 transition-colors"
            >
              <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
                <path fillRule="evenodd" d="M12 2C6.477 2 2 6.484 2 12.017c0 4.425 2.865 8.18 6.839 9.504.5.092.682-.217.682-.483 0-.237-.008-.868-.013-1.703-2.782.605-3.369-1.343-3.369-1.343-.454-1.158-1.11-1.466-1.11-1.466-.908-.62.069-.608.069-.608 1.003.07 1.531 1.032 1.531 1.032.892 1.53 2.341 1.088 2.91.832.092-.647.35-1.088.636-1.338-2.22-.253-4.555-1.113-4.555-4.951 0-1.093.39-1.988 1.029-2.688-.103-.253-.446-1.272.098-2.65 0 0 .84-.27 2.75 1.026A9.564 9.564 0 0112 6.844c.85.004 1.705.115 2.504.337 1.909-1.296 2.747-1.027 2.747-1.027.546 1.379.202 2.398.1 2.651.64.7 1.028 1.595 1.028 2.688 0 3.848-2.339 4.695-4.566 4.943.359.309.678.92.678 1.855 0 1.338-.012 2.419-.012 2.747 0 .268.18.58.688.482A10.019 10.019 0 0022 12.017C22 6.484 17.522 2 12 2z" clipRule="evenodd" />
              </svg>
            </a>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="px-4 py-8">
        <div className="max-w-4xl mx-auto">
          {/* Hero Text - only show when no results */}
          {!result && (
            <div className="text-center mb-10 animate-fade-in">
              <h2 className="text-4xl md:text-5xl font-bold text-charcoal dark:text-gray-100 mb-4">
                Make Every Weekend Count
              </h2>
              <p className="text-xl text-gray-600 dark:text-gray-400 max-w-2xl mx-auto">
                Life is short. Discover how many weekends you have left and get inspired to live each one to the fullest.
              </p>
            </div>
          )}

          {/* Error Message */}
          {error && (
            <div className="max-w-xl mx-auto mb-6 p-4 bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 rounded-xl text-red-700 dark:text-red-300 text-center animate-fade-in">
              <p className="font-medium">Oops! Something went wrong</p>
              <p className="text-sm mt-1">{error}</p>
            </div>
          )}

          {/* Calculator or Results */}
          {isLoading ? (
            <ResultsSkeleton />
          ) : result && lastRequest ? (
            <Results data={result} age={lastRequest.age} onReset={handleReset} />
          ) : (
            <Calculator onCalculate={handleCalculate} isLoading={isLoading} />
          )}
        </div>
      </main>

      {/* Footer */}
      <footer className="py-8 px-4 text-center text-gray-500 dark:text-gray-500 text-sm">
        <p>
          Data provided by{' '}
          <a
            href="https://population.io"
            target="_blank"
            rel="noopener noreferrer"
            className="text-ocean-500 hover:underline"
          >
            population.io
          </a>
          {' '}• Built with ❤️ to inspire you
          {apiVersion && <span> • API v{apiVersion}</span>}
        </p>
      </footer>
    </div>
  );
}

export default App;
