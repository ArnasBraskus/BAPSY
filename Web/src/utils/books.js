export function chooseColor(plan) {
  const BOOK_COLORS = [
    '#597E52',
    '#9b4444',
    '#b83535',
    '#3d5a1f',
    '#A00000'
  ];

  return BOOK_COLORS[plan.id % BOOK_COLORS.length];
}
