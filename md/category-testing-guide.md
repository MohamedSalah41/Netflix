# Category Views Testing Guide 🧪

Complete step-by-step guide to test all Category controller views.

---

## Prerequisites ✅

Before testing, ensure:

1. **Database is up-to-date**
   ```bash
   dotnet ef database update
   ```

2. **Application builds without errors**
   ```bash
   dotnet build
   ```

3. **You have an Admin account** (CategoryController requires `[Authorize(Roles = "Admin")]`)

---

## Step 1: Start the Application 🚀

### Option A: Using Visual Studio
1. Press **F5** or click the green "Play" button
2. Browser will open automatically

### Option B: Using Command Line
```bash
dotnet run
```

Then open your browser to: **https://localhost:5001** (or the port shown in terminal)

---

## Step 2: Create/Login as Admin User 👤

The CategoryController requires **Admin role**. If you don't have an admin user yet:

### Quick Admin User Creation

1. **Register a new account:**
   - Navigate to `/Identity/Account/Register`
   - Create account (e.g., `admin@test.com` / `Admin@123`)

2. **Manually assign Admin role in database:**

   Open SQL Server Management Studio or use this SQL script:

   ```sql
   -- 1. Check if Admin role exists, create if not
   IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
   BEGIN
       INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
       VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())
   END

   -- 2. Find your user ID
   SELECT Id, Email FROM AspNetUsers WHERE Email = 'admin@test.com'

   -- 3. Assign Admin role to user (replace USER_ID and ROLE_ID)
   DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'admin@test.com')
   DECLARE @RoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin')

   IF NOT EXISTS (SELECT * FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
   BEGIN
       INSERT INTO AspNetUserRoles (UserId, RoleId)
       VALUES (@UserId, @RoleId)
   END
   ```

3. **Logout and login again** to refresh claims

---

## Step 3: Test Index View (List) 📋

### URL
```
https://localhost:5001/Category
```
or
```
https://localhost:5001/Category/Index
```

### What to Test

#### ✅ Visual Elements
- [ ] Page title shows "Categories" with 🏷️ icon
- [ ] "Add Category" button appears in top-right
- [ ] Search box appears with magnifying glass icon
- [ ] Table has columns: #, Name, Actions

#### ✅ Empty State (if no categories exist)
- [ ] Shows large 🏷️ icon
- [ ] Shows message: "No categories yet. Add one to get started."

#### ✅ With Data (after adding categories)
- [ ] Categories display in table rows
- [ ] Each category name shows as a red pill badge
- [ ] Each row has "Edit" (blue) and "Delete" (red) buttons
- [ ] Row count shows "X of X categories"

#### ✅ Search/Filter
1. Type in search box (e.g., "Action")
2. [ ] Table filters instantly (no page reload)
3. [ ] Row count updates (e.g., "1 of 5 categories")
4. [ ] Clear search shows all rows again

#### ✅ Hover Effects
- [ ] Table rows slide right slightly on hover
- [ ] Background changes on hover
- [ ] Action buttons change color on hover

---

## Step 4: Test Add View (Create) ➕

### URL
Click "Add Category" button or navigate to:
```
https://localhost:5001/Category/Add
```

### What to Test

#### ✅ Visual Elements
- [ ] Page title shows "Add Category" with 🏷️ icon
- [ ] "Back to Categories" link appears at top
- [ ] Form has single field: "Name"
- [ ] Hint text shows "Max 20 characters"
- [ ] Character counter shows "0 / 20"
- [ ] "Cancel" and "Save Category" buttons at bottom

#### ✅ Character Counter
1. Start typing in Name field
2. [ ] Counter updates live (e.g., "6 / 20")
3. [ ] At 16+ characters, counter turns **orange**
4. [ ] At 20 characters, counter turns **red**

#### ✅ Validation — Empty Field
1. Leave Name field empty
2. Click "Save Category"
3. [ ] Red error message appears: "this Field Is Required"
4. [ ] Form does NOT submit
5. [ ] You stay on the Add page

#### ✅ Validation — Too Long
1. Try to type more than 20 characters
2. [ ] Input stops at 20 characters (maxlength attribute)

#### ✅ Successful Creation
1. Enter valid name (e.g., "Action")
2. Click "Save Category"
3. [ ] Redirects to Index page
4. [ ] **Green success alert** appears: "Category added successfully."
5. [ ] New category appears in table

#### ✅ Cancel Button
1. Click "Cancel" button
2. [ ] Returns to Index page without saving

---

## Step 5: Test Update View (Edit) ✏️

### URL
Click "Edit" button on any category row, or:
```
https://localhost:5001/Category/Update/1
```
(Replace `1` with actual category ID)

### What to Test

#### ✅ Visual Elements
- [ ] Page title shows "Edit Category" with ✏️ icon
- [ ] Icon background is **blue** (not red)
- [ ] Subtitle shows current category name
- [ ] Name field is **pre-filled** with existing value
- [ ] Character counter shows current length
- [ ] "Cancel" and "Save Changes" buttons at bottom
- [ ] "Save Changes" button is **blue** (not red)

#### ✅ Pre-filled Data
- [ ] Name field contains the existing category name
- [ ] Character counter reflects existing name length

#### ✅ Validation — Empty Field
1. Clear the Name field completely
2. Click "Save Changes"
3. [ ] Red error message appears
4. [ ] Form does NOT submit

#### ✅ Successful Update
1. Change name (e.g., "Action" → "Action Movies")
2. Click "Save Changes"
3. [ ] Redirects to Index page
4. [ ] **Green success alert** appears: "Category updated successfully."
5. [ ] Updated name appears in table

#### ✅ Cancel Button
1. Make changes but click "Cancel"
2. [ ] Returns to Index without saving changes

---

## Step 6: Test Delete View (Confirmation) 🗑️

### URL
Click "Delete" button on any category row, or:
```
https://localhost:5001/Category/Delete/1
```

### What to Test

#### ✅ Visual Elements
- [ ] Page title shows "Delete Category" with ⚠️ icon
- [ ] Icon background is **red/pink**
- [ ] Warning box appears with 🚨 icon
- [ ] Warning text shows category name in bold
- [ ] Details panel shows ID and Name
- [ ] Category name displays as red pill badge
- [ ] "Cancel" and "Yes, Delete Category" buttons at bottom
- [ ] Delete button is **red**

#### ✅ Warning Message
- [ ] Shows: "Are you sure you want to delete **[CategoryName]**?"
- [ ] Mentions "This action cannot be undone"
- [ ] Warns about potential FK constraint failures

#### ✅ Successful Deletion
1. Click "Yes, Delete Category"
2. [ ] Redirects to Index page
3. [ ] **Green success alert** appears: "Category deleted successfully."
4. [ ] Category is removed from table

#### ✅ Cancel Button
1. Click "Cancel"
2. [ ] Returns to Index without deleting

---

## Step 7: Test Edge Cases 🔍

### Test 1: Duplicate Names (if validation added)
1. Add category "Drama"
2. Try to add another "Drama"
3. [ ] Should show error (if duplicate check implemented)

### Test 2: Delete In-Use Category
1. Assign a category to a movie/series
2. Try to delete that category
3. [ ] Should fail with FK constraint error OR show custom error message

### Test 3: Invalid ID in URL
Navigate to:
```
https://localhost:5001/Category/Update/99999
```
- [ ] Should show 404 or "Not Found" page

### Test 4: Non-Admin Access
1. Logout
2. Try to access `/Category`
3. [ ] Should redirect to login page
4. After login as non-admin user:
5. [ ] Should show "Access Denied" page

---

## Step 8: Test Responsive Design 📱

### Desktop (1920px)
- [ ] All elements visible and properly spaced
- [ ] Table fits comfortably
- [ ] No horizontal scroll

### Tablet (768px)
- [ ] Layout adjusts gracefully
- [ ] Padding reduces to 20px
- [ ] Table may scroll horizontally

### Mobile (375px)
- [ ] Form buttons stack vertically
- [ ] Search box takes full width
- [ ] Table scrolls horizontally
- [ ] All text remains readable

**How to test:**
- Press **F12** in browser
- Click device toolbar icon
- Select different screen sizes

---

## Step 9: Test Browser Compatibility 🌐

Test in multiple browsers:

- [ ] **Chrome** — primary target
- [ ] **Edge** — should work identically
- [ ] **Firefox** — check CSS variable support
- [ ] **Safari** — check backdrop-filter (navbar blur)

---

## Step 10: Test JavaScript Features 🎯

### Index Page — Table Filter
1. Open browser DevTools (F12) → Console tab
2. Type in search box
3. [ ] No JavaScript errors in console
4. [ ] Filter works instantly
5. [ ] Row count updates correctly

### Add/Update Pages — Character Counter
1. Open DevTools → Console
2. Type in Name field
3. [ ] No JavaScript errors
4. [ ] Counter updates on every keystroke
5. [ ] Colors change at 16 and 20 characters

---

## Common Issues & Solutions 🔧

### Issue 1: "Access Denied" on all Category pages
**Cause:** User is not in Admin role  
**Solution:** Follow Step 2 to assign Admin role

### Issue 2: Styles look broken (no colors, wrong fonts)
**Cause:** `_Layout.cshtml` CSS variables not loading  
**Solution:** 
- Check `Views/_ViewStart.cshtml` sets `Layout = "_Layout"`
- Hard refresh browser (Ctrl+Shift+R)

### Issue 3: Validation doesn't work
**Cause:** `_ValidationScriptsPartial` not found  
**Solution:** Check `Views/Shared/_ValidationScriptsPartial.cshtml` exists

### Issue 4: Character counter doesn't update
**Cause:** JavaScript not loading  
**Solution:** 
- Check browser console for errors
- Ensure `@section Scripts` is in the view

### Issue 5: Database connection error
**Cause:** Connection string invalid or DB not created  
**Solution:**
```bash
dotnet ef database update
```

### Issue 6: "The page cannot be found" (404)
**Cause:** Route mismatch  
**Solution:** Ensure controller is named `CategoryController` and views are in `Views/Category/`

---

## Quick Test Checklist ✅

Use this for rapid testing:

```
□ App starts without errors
□ Can login as Admin
□ Index page loads
□ Add page loads
□ Can create new category
□ Success message appears
□ New category shows in list
□ Search filter works
□ Edit page loads with pre-filled data
□ Can update category
□ Delete confirmation page loads
□ Can delete category
□ All buttons work
□ No JavaScript console errors
□ Mobile layout works
```

---

## Sample Test Data 📝

Use these categories for testing:

1. **Action**
2. **Drama**
3. **Comedy**
4. **Thriller**
5. **Horror**
6. **Sci-Fi**
7. **Romance**
8. **Documentary**
9. **Animation**
10. **Crime**

This gives you enough data to test:
- Pagination (if added later)
- Search filtering
- Sorting
- Delete operations

---

## Performance Testing (Optional) ⚡

### Load Test
1. Add 50+ categories
2. [ ] Index page loads quickly (< 1 second)
3. [ ] Search filter remains responsive
4. [ ] No lag when typing

### Memory Test
1. Open DevTools → Performance tab
2. Record while using all CRUD operations
3. [ ] No memory leaks
4. [ ] Smooth 60fps animations

---

## Automated Testing (Future) 🤖

For CI/CD pipelines, consider adding:

### Unit Tests
```csharp
[Fact]
public async Task Index_ReturnsViewWithCategories()
{
    // Arrange
    var controller = new CategoryController(_context);
    
    // Act
    var result = await controller.Index();
    
    // Assert
    var viewResult = Assert.IsType<ViewResult>(result);
    var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.Model);
}
```

### Integration Tests
```csharp
[Fact]
public async Task Add_ValidCategory_RedirectsToIndex()
{
    var category = new Category { Name = "Test" };
    var response = await _client.PostAsync("/Category/Add", 
        new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("Name", category.Name)
        }));
    
    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
}
```

---

## Final Verification ✅

Before marking as complete:

1. [ ] All 4 views load without errors
2. [ ] All CRUD operations work
3. [ ] Validation works (client + server)
4. [ ] Success/error messages display
5. [ ] Styles match Actor views
6. [ ] JavaScript features work
7. [ ] Mobile responsive
8. [ ] No console errors
9. [ ] Admin authorization enforced
10. [ ] Database updates correctly

---

## Next Steps After Testing 🚀

Once testing is complete:

1. **Fix any bugs found**
2. **Add optional enhancements** (see `category-views-completed.md`)
3. **Document any custom behavior**
4. **Deploy to staging environment**
5. **User acceptance testing**

---

## Support 💬

If you encounter issues:

1. Check browser console (F12) for JavaScript errors
2. Check application logs for server errors
3. Verify database connection
4. Ensure migrations are up-to-date
5. Check user has Admin role

Happy Testing! 🎉
