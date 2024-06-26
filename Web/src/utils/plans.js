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

export async function addPlan(title, author, cover, pages, deadline, weekdays, timeOfDay) {
  const res = await apiDoPost('/api/bookplan/add', {
    title: title,
    author: author,
    cover: cover,
    pages: pages,
    deadline: deadline,
    weekdays: weekdays,
    timeOfDay: timeOfDay
  });

  return res.status == 200;
}

export async function removePlan(id) {
  const res = await apiDoPost('/api/bookplan/remove', {
    id: id
  });

  return res.status == 200;
}

export async function editPlan(id, title, author, cover, pages, deadline, weekdays, timeOfDay) {
  const res = await apiDoPost('/api/bookplan/edit', {
    id: id,
    title: title,
    author: author,
    cover: cover,
    pages: pages,
    deadline: deadline,
    weekdays: weekdays,
    timeOfDay: timeOfDay
  });

  return res.status == 200;
}

export async function additionalPagesRead(id, addPagesRead) {
  const res = await apiDoPost('/api/bookplan/additionalPages', {
    planId: id,
    additionalPagesRead:  addPagesRead
  });

  return res.status == 200;
}

