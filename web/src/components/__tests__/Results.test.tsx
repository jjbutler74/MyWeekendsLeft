import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { Results } from '../Results';
import { WeekendsLeftResponse } from '../../types/api';

const mockData: WeekendsLeftResponse = {
  estimatedWeekendsLeft: 1500,
  estimatedAgeOfDeath: 80,
  estimatedDayOfDeath: '2055-06-15',
  message: 'Make every weekend count!',
  errors: null,
};

describe('Results', () => {
  it('renders the weekend count', async () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    // The count animates up, so wait for the final value
    await waitFor(
      () => {
        expect(screen.getByText('1,500')).toBeInTheDocument();
      },
      { timeout: 3000 }
    );
  });

  it('shows "weekends remaining" label', () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    expect(screen.getByText('weekends remaining')).toBeInTheDocument();
  });

  it('displays the progress bar with lived and left counts', () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    expect(screen.getByText(/2,652 lived/)).toBeInTheDocument();
    expect(screen.getByText(/1,500 left/)).toBeInTheDocument();
  });

  it('shows details after animation completes', async () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    await waitFor(
      () => {
        expect(screen.getByText(/June 15, 2055/)).toBeInTheDocument();
      },
      { timeout: 3000 }
    );

    expect(screen.getByText(/Make every weekend count!/)).toBeInTheDocument();
  });

  it('shows share and calculate again buttons after animation', async () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    await waitFor(
      () => {
        expect(screen.getByText('Share')).toBeInTheDocument();
      },
      { timeout: 3000 }
    );

    expect(screen.getByText('Calculate Again')).toBeInTheDocument();
  });

  it('calls onReset when Calculate Again is clicked', async () => {
    const user = userEvent.setup();
    const onReset = vi.fn();
    render(<Results data={mockData} age={51} onReset={onReset} />);

    await waitFor(
      () => {
        expect(screen.getByText('Calculate Again')).toBeInTheDocument();
      },
      { timeout: 3000 }
    );

    await user.click(screen.getByText('Calculate Again'));
    expect(onReset).toHaveBeenCalledOnce();
  });

  it('shows inspirational tips after animation', async () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    await waitFor(
      () => {
        expect(screen.getByText('Make your weekends count:')).toBeInTheDocument();
      },
      { timeout: 3000 }
    );
  });

  it('renders the WeekendDots visualization', async () => {
    render(<Results data={mockData} age={51} onReset={vi.fn()} />);

    await waitFor(
      () => {
        expect(screen.getByText('Your Life in Weekends')).toBeInTheDocument();
      },
      { timeout: 3000 }
    );
  });
});
