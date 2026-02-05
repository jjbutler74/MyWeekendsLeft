import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { Calculator } from '../Calculator';

describe('Calculator', () => {
  it('renders the form with all fields', () => {
    render(<Calculator onCalculate={vi.fn()} isLoading={false} />);

    expect(screen.getByText('How many weekends do you have left?')).toBeInTheDocument();
    expect(screen.getByLabelText('Your Age')).toBeInTheDocument();
    expect(screen.getByText('Male')).toBeInTheDocument();
    expect(screen.getByText('Female')).toBeInTheDocument();
    expect(screen.getByLabelText('Country')).toBeInTheDocument();
    expect(screen.getByText('Calculate My Weekends')).toBeInTheDocument();
  });

  it('shows loading state when isLoading is true', () => {
    render(<Calculator onCalculate={vi.fn()} isLoading={true} />);

    expect(screen.getByText('Calculating...')).toBeInTheDocument();
    expect(screen.queryByText('Calculate My Weekends')).not.toBeInTheDocument();
  });

  it('calls onCalculate with form values on submit', async () => {
    const user = userEvent.setup();
    const onCalculate = vi.fn();
    render(<Calculator onCalculate={onCalculate} isLoading={false} />);

    await user.click(screen.getByText('Calculate My Weekends'));

    expect(onCalculate).toHaveBeenCalledWith({
      age: 35,
      gender: 'Male',
      country: 'USA',
    });
  });

  it('allows selecting Female gender', async () => {
    const user = userEvent.setup();
    const onCalculate = vi.fn();
    render(<Calculator onCalculate={onCalculate} isLoading={false} />);

    await user.click(screen.getByText('Female'));
    await user.click(screen.getByText('Calculate My Weekends'));

    expect(onCalculate).toHaveBeenCalledWith(
      expect.objectContaining({ gender: 'Female' })
    );
  });

  it('disables submit button when loading', () => {
    render(<Calculator onCalculate={vi.fn()} isLoading={true} />);

    const submitButton = screen.getByRole('button', { name: /calculating/i });
    expect(submitButton).toBeDisabled();
  });

  it('renders all country options', () => {
    render(<Calculator onCalculate={vi.fn()} isLoading={false} />);

    const select = screen.getByLabelText('Country');
    expect(select).toBeInTheDocument();
    // Check a few key countries exist
    expect(screen.getByText(/United States/)).toBeInTheDocument();
    expect(screen.getByText(/United Kingdom/)).toBeInTheDocument();
    expect(screen.getByText(/Japan/)).toBeInTheDocument();
  });
});
