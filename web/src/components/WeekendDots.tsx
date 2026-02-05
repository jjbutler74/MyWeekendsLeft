import { useMemo } from 'react';

interface WeekendDotsProps {
  weekendsLived: number;
  weekendsLeft: number;
}

export function WeekendDots({ weekendsLived, weekendsLeft }: WeekendDotsProps) {
  const totalWeekends = weekendsLived + weekendsLeft;

  // Show a representative sample - max ~1000 dots for performance
  const scale = useMemo(() => {
    if (totalWeekends <= 1000) return 1;
    return Math.ceil(totalWeekends / 1000);
  }, [totalWeekends]);

  const scaledLived = Math.round(weekendsLived / scale);
  const scaledLeft = Math.round(weekendsLeft / scale);
  const scaledTotal = scaledLived + scaledLeft;

  const dots = useMemo(() => {
    const arr = [];
    for (let i = 0; i < scaledTotal; i++) {
      arr.push(i < scaledLived ? 'lived' : 'left');
    }
    return arr;
  }, [scaledLived, scaledTotal]);

  return (
    <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-6 animate-slide-up">
      <h3 className="font-semibold text-charcoal dark:text-gray-200 mb-1 text-center">
        Your Life in Weekends
      </h3>
      <p className="text-xs text-gray-500 dark:text-gray-400 text-center mb-4">
        {scale > 1 ? `Each dot = ${scale} weekends` : 'Each dot = 1 weekend'}
      </p>
      <div className="flex flex-wrap gap-[3px] justify-center">
        {dots.map((type, i) => (
          <div
            key={i}
            className={`w-[5px] h-[5px] rounded-full ${
              type === 'lived'
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
