import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function parseUtcDate(dateValue?: string): Date | null {
  if (!dateValue) return null;
  const trimmed = dateValue.trim();
  if (!trimmed) return null;

  // ISO strings without timezone offset ('Z' or '+00:00') are parsed by JS as local time.
  // Normalize strings lacking timezone info to UTC ('Z') so Date treats them as UTC before converting to local time.
  const hasTimezone = /[Z+-]\d{2}:?\d{2}$|Z$/i.test(trimmed);
  const normalizedString = hasTimezone ? trimmed : `${trimmed}Z`;
  const date = new Date(normalizedString);
  return Number.isNaN(date.getTime()) ? null : date;
}

export function formatGameDate(dateValue?: string): string {
  const date = parseUtcDate(dateValue);
  if (!date) return "Unknown date";
  return date.toLocaleString();
}
