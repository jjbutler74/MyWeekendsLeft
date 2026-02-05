import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { WeekendDots } from '../WeekendDots';

describe('WeekendDots', () => {
  it('renders the correct total count', () => {
    render(<WeekendDots weekendsLived={1000} weekendsLeft={500} />);

    expect(screen.getByText('Your Life in Weekends')).toBeInTheDocument();
    expect(screen.getByText(/1,500 total/)).toBeInTheDocument();
  });

  it('always shows 1 dot = 1 weekend', () => {
    render(<WeekendDots weekendsLived={2600} weekendsLeft={1500} />);

    expect(screen.getByText('Each dot = 1 weekend (4,100 total)')).toBeInTheDocument();
  });

  it('renders exact number of dots matching total weekends', () => {
    const weekendsLived = 100;
    const weekendsLeft = 50;
    const { container } = render(
      <WeekendDots weekendsLived={weekendsLived} weekendsLeft={weekendsLeft} />
    );

    const dots = container.querySelectorAll('.rounded-full.w-\\[3px\\]');
    expect(dots.length).toBe(150);
  });

  it('shows lived and remaining counts in legend', () => {
    render(<WeekendDots weekendsLived={2652} weekendsLeft={1348} />);

    expect(screen.getByText(/Lived \(2,652\)/)).toBeInTheDocument();
    expect(screen.getByText(/Remaining \(1,348\)/)).toBeInTheDocument();
  });

  it('renders lived dots before remaining dots', () => {
    const weekendsLived = 3;
    const weekendsLeft = 2;
    const { container } = render(
      <WeekendDots weekendsLived={weekendsLived} weekendsLeft={weekendsLeft} />
    );

    const dots = container.querySelectorAll('.rounded-full.w-\\[3px\\]');
    // First 3 should be "lived" (gray), last 2 should be "left" (sunset)
    expect(dots[0]).toHaveClass('bg-gray-300');
    expect(dots[1]).toHaveClass('bg-gray-300');
    expect(dots[2]).toHaveClass('bg-gray-300');
    expect(dots[3]).toHaveClass('bg-sunset-500');
    expect(dots[4]).toHaveClass('bg-sunset-500');
  });
});
