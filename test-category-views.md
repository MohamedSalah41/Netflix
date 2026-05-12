# Quick Start: Test Category Views 🚀

## 1️⃣ Start the Application

```bash
dotnet run
```

Wait for: `Now listening on: https://localhost:XXXX`

## 2️⃣ Open Browser

Navigate to: **https://localhost:5001/Category**

(Or whatever port is shown in the terminal)

---

## 3️⃣ If You See "Access Denied" ⚠️

The CategoryController requires **Admin role**. You need to:

### Option A: Quick SQL Fix (Fastest)

Run this SQL in your database:

```sql
-- Create Admin role if it doesn't exist
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())
END

-- Assign Admin role to your user (replace email)
DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'YOUR_EMAIL@example.com')
DECLARE @RoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin')

IF @UserId IS NOT NULL AND @RoleId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @RoleId)
    END
END
```

**Then logout and login again.**

### Option B: Register New Admin User

1. Go to `/Identity/Account/Register`
2. Create account: `admin@test.com` / `Admin@123`
3. Run SQL above with this email
4. Logout and login again

---

## 4️⃣ Test All Views

### ✅ Index (List)
**URL:** `/Category`

- Should show empty state or list of categories
- Search box should filter instantly
- "Add Category" button should be visible

### ✅ Add (Create)
**URL:** `/Category/Add`

1. Enter name: "Action"
2. Watch character counter update
3. Click "Save Category"
4. Should redirect to Index with green success message

### ✅ Update (Edit)
**URL:** Click "Edit" on any category

1. Name should be pre-filled
2. Change it (e.g., "Action Movies")
3. Click "Save Changes"
4. Should redirect with success message

### ✅ Delete
**URL:** Click "Delete" on any category

1. Should show warning with category name
2. Click "Yes, Delete Category"
3. Should redirect with success message
4. Category should be gone from list

---

## 5️⃣ Quick Visual Check ✨

Open DevTools (F12) and check:

- **Console tab:** No red errors
- **Network tab:** All requests return 200 OK
- **Responsive mode:** Test mobile view (375px width)

---

## 🎯 Expected Results

| Action | Expected Result |
|--------|----------------|
| Navigate to `/Category` | Shows list or empty state |
| Click "Add Category" | Opens form with character counter |
| Submit empty form | Shows validation error |
| Submit valid form | Redirects with green success alert |
| Click "Edit" | Opens pre-filled form |
| Update category | Saves and shows success message |
| Click "Delete" | Shows confirmation page |
| Confirm delete | Removes category and shows success |
| Type in search box | Filters table instantly |
| Hover table rows | Row slides right, background changes |

---

## 🐛 Common Issues

### "Access Denied"
→ User not in Admin role (see step 3)

### Styles look broken
→ Hard refresh: `Ctrl + Shift + R`

### Character counter doesn't work
→ Check browser console for JavaScript errors

### "The page cannot be found"
→ Ensure app is running and URL is correct

---

## 📸 What You Should See

### Index Page
```
┌─────────────────────────────────────────────┐
│ 🏷️  Categories                [+ Add Category] │
├─────────────────────────────────────────────┤
│ 🔍 Filter categories...        5 of 5 categories│
├─────────────────────────────────────────────┤
│ #  │ Name        │ Actions                  │
├────┼─────────────┼──────────────────────────┤
│ 1  │ [Action]    │ [Edit] [Delete]          │
│ 2  │ [Drama]     │ [Edit] [Delete]          │
│ 3  │ [Comedy]    │ [Edit] [Delete]          │
└─────────────────────────────────────────────┘
```

### Add Page
```
┌─────────────────────────────────────────────┐
│ ← Back to Categories                         │
│                                              │
│ ┌──────────────────────────────────────────┐│
│ │ 🏷️  Add Category                          ││
│ │ Fill in the details below...             ││
│ ├──────────────────────────────────────────┤│
│ │ NAME                                     ││
│ │ Max 20 characters                        ││
│ │ [Action________________]                 ││
│ │                              6 / 20      ││
│ ├──────────────────────────────────────────┤│
│ │              [Cancel] [Save Category]    ││
│ └──────────────────────────────────────────┘│
└─────────────────────────────────────────────┘
```

---

## ✅ Success Criteria

You're done when:

- [x] All 4 pages load without errors
- [x] Can create new category
- [x] Can edit existing category
- [x] Can delete category
- [x] Search filter works
- [x] Character counter updates
- [x] Success messages appear
- [x] No console errors

---

## 🎉 That's It!

Your Category views are working if all the above tests pass.

For detailed testing scenarios, see: `md/category-testing-guide.md`
