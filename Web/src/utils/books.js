import { getImage } from './resources.js';

export function chooseColor(plan) {
  const BOOK_COLORS = [
    '#597E52',
    '#9b4444',
    '#b83535',
    '#3d5a1f',
    '#A00000'
  ];

  if (plan.cover != "_none")
    return `url('${getImage(plan.cover)}')`;

  return BOOK_COLORS[plan.id % BOOK_COLORS.length];
}
