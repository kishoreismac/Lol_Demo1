const { test, expect } = require('@playwright/test');

async function updateQaBoard(request, caseId, title, status, notes) {
  await request.post('/qa/state', {
    data: {
      caseId,
      title,
      status,
      notes
    }
  });
}

async function runTrackedCase(request, caseId, title, body) {
  await updateQaBoard(request, caseId, title, 'running', 'Executing smoke test in headed Chromium.');

  try {
    await body();
    await updateQaBoard(request, caseId, title, 'passed', 'Smoke test passed.');
  } catch (error) {
    await updateQaBoard(request, caseId, title, 'failed', error.message);
    throw error;
  }
}

test('TC-UI-001 home page loads successfully', async ({ page, request }) => {
  await runTrackedCase(request, 'TC-UI-001', 'Home page loads successfully', async () => {
    await page.goto('/');

    await expect(page.getByRole('heading', { name: /clean dairy catalog demo/i })).toBeVisible();
    await expect(page.getByText('Catalog favorites')).toBeVisible();
    await expect(page.getByRole('link', { name: 'Explore Products' })).toBeVisible();
  });
});

test('TC-UI-002 products page loads successfully', async ({ page, request }) => {
  await runTrackedCase(request, 'TC-UI-002', 'Products page loads successfully', async () => {
    await page.goto('/Products');

    await expect(page.getByRole('heading', { name: 'Browse dairy products' })).toBeVisible();
    await expect(page.getByLabel('Search by product name')).toBeVisible();
    await expect(page.getByLabel('Category')).toBeVisible();
  });
});

test('TC-UI-003 search by product name returns matching items', async ({ page, request }) => {
  await runTrackedCase(request, 'TC-UI-003', 'Search by product name returns matching items', async () => {
    await page.goto('/Products');
    await page.getByLabel('Search by product name').fill('Butter');
    await page.getByRole('button', { name: 'Apply' }).click();

    await expect(page.getByRole('heading', { name: 'Salted Butter', exact: true })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Swiss Cheese' })).toHaveCount(0);
  });
});

test('TC-UI-004 category filter narrows the catalog', async ({ page, request }) => {
  await runTrackedCase(request, 'TC-UI-004', 'Category filter narrows the catalog', async () => {
    await page.goto('/Products');
    await page.getByLabel('Category').selectOption('Cheese');
    await page.getByRole('button', { name: 'Apply' }).click();

    await expect(page.getByRole('heading', { name: 'Swiss Cheese' })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Salted Butter' })).toHaveCount(0);
  });
});

test('TC-UI-005 product details page opens from the catalog', async ({ page, request }) => {
  await runTrackedCase(request, 'TC-UI-005', 'Product details page opens from the catalog', async () => {
    await page.goto('/Products');
    await page.locator('.product-card').filter({ has: page.getByRole('heading', { name: 'Salted Butter', exact: true }) }).getByRole('link', { name: 'View Details' }).click();

    await expect(page).toHaveURL(/\/Products\/Details\//);
    await expect(page.getByRole('heading', { name: 'Salted Butter', exact: true })).toBeVisible();
    await expect(page.getByText('Product ID: LLD001')).toBeVisible();
  });
});

test('TC-UI-006 admin create, edit, and delete flow works', async ({ page, request }) => {
  await runTrackedCase(request, 'TC-UI-006', 'Admin create, edit, and delete flow works', async () => {
    const suffix = 800 + (Date.now() % 180);
    const productId = `LLD${String(suffix).padStart(3, '0')}`;
    const baseName = `QA Demo Butter ${suffix}`;
    const updatedName = `${baseName} Updated`;

    await page.goto('/AdminProducts/Create');
    await page.getByLabel('Product ID').fill(productId);
    await page.getByLabel('Product Name').fill(baseName);
    await page.getByLabel('Category').selectOption('Butter & Spreads');
    await page.getByLabel('Brand').fill('Land O Lakes');
    await page.getByLabel('Short Description').fill('Created by Playwright smoke testing.');
    await page.getByLabel('Package Size').fill('8 oz tub');
    await page.getByLabel('Price').fill('4.89');
    await page.getByLabel('Image File Name').fill('qa-demo-butter.jpg');
    await page.getByLabel('Tags').fill('butter, qa, smoke');
    await page.getByRole('button', { name: 'Save Product' }).click();

    await expect(page.getByText(`${baseName} was added to the catalog.`)).toBeVisible();
    await page.getByLabel('Search by product name').fill(baseName);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.getByText(baseName)).toBeVisible();

    await page.getByRole('link', { name: 'Edit' }).click();
    await page.getByLabel('Product Name').fill(updatedName);
    await page.getByLabel('Short Description').fill('Updated by Playwright smoke testing.');
    await page.getByLabel('Price').fill('4.99');
    await page.getByRole('button', { name: 'Save Changes' }).click();

    await expect(page.getByText(`${updatedName} was updated.`)).toBeVisible();
    await page.getByLabel('Search by product name').fill(updatedName);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.getByText(updatedName)).toBeVisible();

    await page.getByRole('link', { name: 'Delete' }).click();
    await expect(page.getByRole('heading', { name: 'Delete product' })).toBeVisible();
    await page.getByRole('button', { name: 'Delete Product' }).click();

    await expect(page.getByText('The product was removed from the catalog.')).toBeVisible();
    await page.getByLabel('Search by product name').fill(updatedName);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.getByText(updatedName)).toHaveCount(0);
  });
});