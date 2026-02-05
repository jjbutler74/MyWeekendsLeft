import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { DarkModeToggle } from '../DarkModeToggle';

describe('DarkModeToggle', () => {
  it('renders with light mode label when isDark is false', () => {
    render(<DarkModeToggle isDark={false} onToggle={vi.fn()} />);

    expect(screen.getByLabelText('Switch to dark mode')).toBeInTheDocument();
  });

  it('renders with dark mode label when isDark is true', () => {
    render(<DarkModeToggle isDark={true} onToggle={vi.fn()} />);

    expect(screen.getByLabelText('Switch to light mode')).toBeInTheDocument();
  });

  it('calls onToggle when clicked', async () => {
    const user = userEvent.setup();
    const onToggle = vi.fn();
    render(<DarkModeToggle isDark={false} onToggle={onToggle} />);

    await user.click(screen.getByRole('button'));

    expect(onToggle).toHaveBeenCalledOnce();
  });
});
