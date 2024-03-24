import { apiDoGet, apiDoPost } from './api.js';

export async function getPlan(id) {
  const res = await apiDoGet(`/api/bookplan/get/${id}`);
  const data = await res.json();

  return data;
}

export async function getPlans() {
  const res = await apiDoGet('/api/bookplan/list');
  const data = await res.json();

  let plans = [];

  for (const id of data.ids) {
    plans.push(await getPlan(id));
  }

  return plans;
}

export async function addPlan(title, author, pages, deadline, weekdays, timeOfDay) {
  const res = await apiDoPost('/api/bookplan/add', {
    title: title,
    author: author,
    pages: pages,
    deadline: deadline,
    weekdays: weekdays,
    timeOfDay: timeOfDay,
    size: 0
  });

  return res.status == 200;
}

export async function removePlan(id) {
  const res = await apiDoPost('/api/bookplan/remove', {
    id: id
  });

  return res.status == 200;
}

export async function editPlan(id, title, author, pages, deadline, weekdays, timeOfDay) {
  const res = await apiDoPost('/api/bookplan/edit', {
    id: id,
    title: title,
    author: author,
    pages: pages,
    deadline: deadline,
    weekdays: weekdays,
    timeOfDay: timeOfDay,
    size: 0
  });

  return res.status == 200;
}
