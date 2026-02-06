interface WeekendDotsProps {
  weekendsLived: number;
  weekendsLeft: number;
}

export function WeekendDots({ weekendsLived, weekendsLeft }: WeekendDotsProps) {
  const totalWeekends = weekendsLived + weekendsLeft;

  const livedCount = weekendsLived;

  return (
    <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-6 animate-slide-up">
      <h3 className="font-semibold text-charcoal dark:text-gray-200 mb-1 text-center">
        Your Life in Weekends
      </h3>
      <p className="text-xs text-gray-500 dark:text-gray-400 text-center mb-4">
        Each dot = 1 weekend ({totalWeekends.toLocaleString()} total)
      </p>
      <div className="flex flex-wrap gap-px justify-center">
        {Array.from({ length: totalWeekends }, (_, i) => (
          <div
            key={i}
            className={`w-[3px] h-[3px] rounded-full ${
              i < livedCount
                ? 'bg-gray-300 dark:bg-gray-600'
                : 'bg-sunset-500'
            }`}
          />
        ))}
      </div>
      <div className="flex justify-center gap-6 mt-4 text-xs text-gray-500 dark:text-gray-400">
        <div className="flex items-center gap-1.5">
          <div className="w-2.5 h-2.5 rounded-full bg-gray-300 dark:bg-gray-600" />
          <span>Lived ({weekendsLived.toLocaleString()})</span>
        </div>
        <div className="flex items-center gap-1.5">
          <div className="w-2.5 h-2.5 rounded-full bg-sunset-500" />
          <span>Remaining ({weekendsLeft.toLocaleString()})</span>
        </div>
      </div>
    </div>
  );
}
