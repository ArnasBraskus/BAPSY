import { expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ModalConfirmPlainCss from '../Components/ModalConfirmPlainCss.vue';

//Test 1
test('BookTitle missing', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: '', // Invalid data
            author: 'Sample Author',
            pages: 10,
            deadline: '2025-03-25',
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Book title required.");
});

//Test 2
test('BookTitle and Author missing', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: '', // Invalid data
            author: '', // Invalid data
            pages: 10,
            deadline: '2025-03-25',
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Book title required.");
    expect(wrapper.vm.errors).toContain("Author required.");
});

//Test 3
test('BookTitle, Author, pages missing', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: '', // Invalid data
            author: '', // Invalid data
            pages: null, //Invalid data 
            deadline: '2025-03-25',
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Book title required.");
    expect(wrapper.vm.errors).toContain("Author required.");
    expect(wrapper.vm.errors).toContain("Page count required.");
});

// Test 4
test('All fields missing', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: '', // Invalid data
            author: '', // Invalid data
            pages: null, // Invalid data 
            deadline: '', // Invalid data
            day1: false,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '' // Invalid data
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Book title required.");
    expect(wrapper.vm.errors).toContain("Author required.");
    expect(wrapper.vm.errors).toContain("Page count required.");
    expect(wrapper.vm.errors).toContain("Deadline date required.");
    expect(wrapper.vm.errors).toContain("At least one day required.");
    expect(wrapper.vm.errors).toContain("Hour required.");
});

// Test 5
test('All fields missing except Day', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: '', // Invalid data
            author: '', // Invalid data
            pages: null, // Invalid data 
            deadline: '', // Invalid data
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '' // Invalid data
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Book title required.");
    expect(wrapper.vm.errors).toContain("Author required.");
    expect(wrapper.vm.errors).toContain("Page count required.");
    expect(wrapper.vm.errors).toContain("Deadline date required.");
    expect(wrapper.vm.errors).toContain("Hour required.");
});

// Test 6
test('BookTitle and Deadline missing', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: '', // Invalid data
            author: 'Sample Author',
            pages: 10,
            deadline: '', // Invalid data
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Book title required.");
    expect(wrapper.vm.errors).toContain("Deadline date required.");
});

// Test 7
test('Page count is negative', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: 'Sample Book',
            author: 'Sample Author',
            pages: -10, // Invalid data
            deadline: '2025-03-25',
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Page count cannot be negative.");
});

// Test 8
test('Deadline date is older than today', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);
    const today = new Date();
    const yesterday = new Date(today);
    yesterday.setDate(today.getDate() - 1); // Set to yesterday

    await wrapper.setData({
        formData: {
            bookTitle: 'Sample Book',
            author: 'Sample Author',
            pages: 10,
            deadline: yesterday.toISOString().split('T')[0], // Invalid data
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Deadline date cannot be older than today.");
});

// Test 9
test('No days selected', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: 'Sample Book',
            author: 'Sample Author',
            pages: 10,
            deadline: '2025-03-25',
            day1: false, // Invalid data
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("At least one day required.");
});

// Test 10
test('Hour is missing', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: 'Sample Book',
            author: 'Sample Author',
            pages: 10,
            deadline: '2025-03-25',
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '' // Invalid data
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toContain("Hour required.");
});

// Test 11
test('All fields are selected', async () => {
    const wrapper = await mount(ModalConfirmPlainCss);

    await wrapper.setData({
        formData: {
            bookTitle: 'Sample Book',
            author: 'Sample Author',
            pages: 10,
            deadline: '2025-03-25',
            day1: true,
            day2: true,
            day3: true,
            day4: true,
            day5: true,
            day6: true,
            day7: true,
            timeOfDay: '10:30 PM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toHaveLength(0);
});

//Test 12
test('Everything in its right place', async () => {
    
    const wrapper = await mount(ModalConfirmPlainCss);
    await wrapper.setData({
        formData: {
            bookTitle: 'Sample Book',
            author: 'Sample Author',
            pages: 100,
            deadline: '2024-03-30',
            day1: true,
            day2: false,
            day3: false,
            day4: false,
            day5: false,
            day6: false,
            day7: false,
            timeOfDay: '10:30 AM'
        }
    });

    await wrapper.vm.checkForm();

    expect(wrapper.vm.errors).toHaveLength(0);
})
