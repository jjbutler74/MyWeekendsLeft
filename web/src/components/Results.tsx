import { useEffect, useState, useMemo } from 'react';
import { WeekendsLeftResponse } from '../types/api';

const ALL_TIPS = [
  "Try something new every month",
  "Spend quality time with loved ones",
  "Take that trip you've been postponing",
  "Disconnect from screens and connect with nature",
  "Learn a new skill or hobby",
  "Have breakfast in bed on Sunday",
  "Watch the sunrise at least once a month",
  "Call someone you haven't spoken to in years",
  "Start a passion project",
  "Go on a spontaneous road trip",
  "Have a picnic in the park",
  "Learn to cook a dish from another culture",
  "Write a letter to your future self",
  "Visit a place you've never been in your own city",
  "Take a digital detox weekend",
  "Volunteer for a cause you care about",
  "Host a game night with friends",
  "Watch the sunset with someone you love",
  "Try a new restaurant every month",
  "Go stargazing away from city lights",
  "Take up gardening or grow your own herbs",
  "Read a book in one sitting",
  "Have a movie marathon of classics you've missed",
  "Go camping under the stars",
  "Learn to play a musical instrument",
  "Take a dance class",
  "Visit a farmers market",
  "Go hiking on a trail you've never explored",
  "Have a spa day at home",
  "Write down 3 things you're grateful for",
  "Reconnect with an old friend over coffee",
  "Try meditation or yoga",
  "Create art, even if you think you can't",
  "Build something with your hands",
  "Teach someone something you know well",
  "Say yes to an invitation you'd normally decline",
  "Take photos of moments, not just places",
  "Have a tech-free dinner with family",
  "Go to a live concert or performance",
  "Explore a museum you've never visited",
  "Plant a tree",
  "Learn a few phrases in a new language",
  "Wake up early to enjoy the quiet morning",
  "Write in a journal about your dreams",
  "Take the scenic route instead of the fastest",
  "Forgive someone who wronged you",
  "Tell people you love them more often",
  "Make a bucket list and start crossing things off",
  "Celebrate small wins",
  "Be present - put your phone away",
];

function getRandomTips(count: number): string[] {
  const shuffled = [...ALL_TIPS].sort(() => Math.random() - 0.5);
  return shuffled.slice(0, count);
}

interface ResultsProps {
  data: WeekendsLeftResponse;
  age: number;
  onReset: () => void;
}

export function Results({ data, age, onReset }: ResultsProps) {
  const [displayedCount, setDisplayedCount] = useState(0);
  const [showDetails, setShowDetails] = useState(false);

  // Get 4 random tips (memoized so they don't change on re-render)
  const randomTips = useMemo(() => getRandomTips(4), []);

  // Animate the count up
  useEffect(() => {
    const target = data.estimatedWeekendsLeft;
    const duration = 1500;
    const steps = 60;
    const increment = target / steps;
    let current = 0;

    const timer = setInterval(() => {
      current += increment;
      if (current >= target) {
        setDisplayedCount(target);
        clearInterval(timer);
        setTimeout(() => setShowDetails(true), 300);
      } else {
        setDisplayedCount(Math.floor(current));
      }
    }, duration / steps);

    return () => clearInterval(timer);
  }, [data.estimatedWeekendsLeft]);

  // Calculate weekends lived (approximate)
  const weekendsLived = age * 52;
  const totalWeekends = weekendsLived + data.estimatedWeekendsLeft;
  const progressPercent = (weekendsLived / totalWeekends) * 100;

  // Format the estimated date
  const deathDate = new Date(data.estimatedDayOfDeath);
  const formattedDate = deathDate.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });

  const handleShare = async () => {
    const text = `I have approximately ${data.estimatedWeekendsLeft.toLocaleString()} weekends left in my life. Time to make them count! 🌅 #MyWeekendsLeft`;

    if (navigator.share) {
      try {
        await navigator.share({ text, url: window.location.href });
      } catch {
        // User cancelled or error
      }
    } else {
      navigator.clipboard.writeText(text);
      alert('Copied to clipboard!');
    }
  };

  return (
    <div className="w-full max-w-xl mx-auto space-y-6 animate-fade-in">
      {/* Main Result Card */}
      <div className="bg-white rounded-2xl shadow-lg p-8 text-center">
        <div className="mb-6">
          <div className="text-7xl md:text-8xl font-extrabold text-sunset-500 animate-count-up">
            {displayedCount.toLocaleString()}
          </div>
          <div className="text-xl text-gray-600 mt-2">
            weekends remaining
          </div>
        </div>

        {/* Progress Bar */}
        <div className="mb-6">
          <div className="h-4 bg-gray-200 rounded-full overflow-hidden">
            <div
              className="h-full bg-gradient-to-r from-ocean-400 to-sunset-500 progress-bar rounded-full"
              style={{ width: `${progressPercent}%` }}
            />
          </div>
          <div className="flex justify-between text-sm text-gray-500 mt-2">
            <span>{weekendsLived.toLocaleString()} lived</span>
            <span>{data.estimatedWeekendsLeft.toLocaleString()} left</span>
          </div>
        </div>

        {/* Details */}
        {showDetails && (
          <div className="space-y-3 text-gray-600 animate-slide-up">
            <div className="flex items-center justify-center gap-2">
              <span className="text-2xl">📅</span>
              <span>
                Estimated until: <strong>{formattedDate}</strong> (age {data.estimatedAgeOfDeath})
              </span>
            </div>
            <p className="text-lg italic text-charcoal mt-4">
              "{data.message}"
            </p>
          </div>
        )}
      </div>

      {/* Action Buttons */}
      {showDetails && (
        <div className="flex flex-col sm:flex-row gap-4 animate-slide-up">
          <button
            onClick={handleShare}
            className="flex-1 py-3 px-6 bg-ocean-400 hover:bg-ocean-500 text-white font-medium rounded-xl shadow-md hover:shadow-lg transition-all flex items-center justify-center gap-2"
          >
            <span>📱</span> Share
          </button>
          <button
            onClick={onReset}
            className="flex-1 py-3 px-6 bg-white hover:bg-gray-50 text-charcoal font-medium rounded-xl shadow-md hover:shadow-lg transition-all border-2 border-gray-200 flex items-center justify-center gap-2"
          >
            <span>🔄</span> Calculate Again
          </button>
        </div>
      )}

      {/* Inspirational Tips */}
      {showDetails && (
        <div className="bg-gradient-to-br from-sunset-50 to-ocean-50 rounded-2xl p-6 animate-slide-up">
          <h3 className="font-semibold text-charcoal mb-3 flex items-center gap-2">
            <span>💡</span> Make your weekends count:
          </h3>
          <ul className="space-y-2 text-gray-700">
            {randomTips.map((tip, index) => (
              <li key={index} className="flex items-start gap-2">
                <span className="text-sunset-500">•</span>
                {tip}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
