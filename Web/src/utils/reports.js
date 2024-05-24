import { apiDoGet } from './api.js';

export async function getReport(id) {
  const res = await apiDoGet(`/api/reports/get/${id}`);
  const data = await res.json();

  return data;
}

export async function getReports() {
  try{
    const res = await apiDoGet('/api/reports/list');
    const data = await res.json();

    let reports = [];

    for (const id of data.ids) {
      reports.push(await getReport(id));
    }

    return reports;
  }
  catch (error) {
    console.error('Error fetching reports:', error);
    throw error;
  }
}