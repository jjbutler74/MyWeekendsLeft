export function ResultsSkeleton() {
  return (
    <div className="w-full max-w-xl mx-auto space-y-6 animate-pulse">
      {/* Main count skeleton */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-8 text-center">
        <div className="h-20 w-48 bg-gray-200 dark:bg-gray-700 rounded-xl mx-auto mb-4" />
        <div className="h-6 w-32 bg-gray-200 dark:bg-gray-700 rounded mx-auto" />
      </div>

      {/* Progress bar skeleton */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-6">
        <div className="h-4 w-full bg-gray-200 dark:bg-gray-700 rounded-full mb-3" />
        <div className="flex justify-between">
          <div className="h-4 w-24 bg-gray-200 dark:bg-gray-700 rounded" />
          <div className="h-4 w-24 bg-gray-200 dark:bg-gray-700 rounded" />
        </div>
      </div>

      {/* Details skeleton */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-6 space-y-4">
        <div className="h-5 w-3/4 bg-gray-200 dark:bg-gray-700 rounded mx-auto" />
        <div className="h-4 w-1/2 bg-gray-200 dark:bg-gray-700 rounded mx-auto" />
        <div className="h-4 w-2/3 bg-gray-200 dark:bg-gray-700 rounded mx-auto" />
      </div>

      {/* Weekend dots skeleton */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-6">
        <div className="h-5 w-40 bg-gray-200 dark:bg-gray-700 rounded mx-auto mb-4" />
        <div className="flex flex-wrap gap-1 justify-center">
          {Array.from({ length: 100 }, (_, i) => (
            <div
              key={i}
              className="w-2 h-2 bg-gray-200 dark:bg-gray-700 rounded-full"
            />
          ))}
        </div>
      </div>

      {/* Tips skeleton */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg p-6">
        <div className="h-5 w-48 bg-gray-200 dark:bg-gray-700 rounded mb-4" />
        <div className="space-y-2">
          <div className="h-4 w-full bg-gray-200 dark:bg-gray-700 rounded" />
          <div className="h-4 w-5/6 bg-gray-200 dark:bg-gray-700 rounded" />
          <div className="h-4 w-4/5 bg-gray-200 dark:bg-gray-700 rounded" />
          <div className="h-4 w-full bg-gray-200 dark:bg-gray-700 rounded" />
        </div>
      </div>
    </div>
  );
}
