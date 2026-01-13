USE ExpenseTrackingDB;
GO

INSERT INTO Categories (CategoryName, Description, IsDefault, IsActive, CreatedDate)
VALUES 
    ('Food & Dining', 'Meals, groceries, restaurants', 1, 1, GETDATE()),
    ('Transportation', 'Fuel, public transport, vehicle maintenance', 1, 1, GETDATE()),
    ('Rent & Utilities', 'Rent, electricity, water, internet', 1, 1, GETDATE()),
    ('Office Supplies', 'Stationery, equipment, consumables', 1, 1, GETDATE()),
    ('Salaries & Wages', 'Employee compensation', 1, 1, GETDATE()),
    ('Marketing', 'Advertising, promotions, campaigns', 1, 1, GETDATE()),
    ('Maintenance', 'Repairs, upkeep, servicing', 1, 1, GETDATE()),
    ('Insurance', 'Business insurance premiums', 1, 1, GETDATE()),
    ('Professional Services', 'Legal, accounting, consulting', 1, 1, GETDATE()),
    ('Miscellaneous', 'Other expenses', 1, 1, GETDATE());
GO