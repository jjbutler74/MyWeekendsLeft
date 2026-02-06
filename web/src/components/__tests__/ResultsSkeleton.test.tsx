import { render } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { ResultsSkeleton } from '../ResultsSkeleton';

describe('ResultsSkeleton', () => {
  it('renders skeleton placeholders', () => {
    const { container } = render(<ResultsSkeleton />);

    // Should have pulse animation
    expect(container.querySelector('.animate-pulse')).toBeInTheDocument();

    // Should render multiple skeleton blocks (cards)
    const cards = container.querySelectorAll('.bg-white');
    expect(cards.length).toBeGreaterThan(3);
  });

  it('renders skeleton dots for weekend visualization', () => {
    const { container } = render(<ResultsSkeleton />);

    // Should have placeholder dots
    const dots = container.querySelectorAll('.rounded-full.w-2.h-2');
    expect(dots.length).toBe(100);
  });
});
