export const getEnrolledStatus = (id: number): string => {
  const enrolledIds = [1, 2, 3, 4];
  return enrolledIds.includes(id) ? "Y" : "N";
};

export const getForfeitedStatus = (id: number): string => {
  const forfeitedIds = [3, 4];
  return forfeitedIds.includes(id) ? "Y" : "N";
};