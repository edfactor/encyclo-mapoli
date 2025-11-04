## Adding a New Page to Navigation

## Step 1: Create route constant

When you have created a new page, which will be a react component, then add a new route constant to ./src/ui/src/constants.ts.

Put it under the ROUTES variable with an ALL_CAPS name with underscores and set it to the relative url.


## Step 2: Register Route

Add it in a new element as a child of the <Routes> component in ./src/ui/src/components/router/RouterSubAssembly.tsx


## Step 3: Adding page to back end navigation

The page must be added through an Oracle PL/SQL script in ./src/database/ready_import/Navigations/add-navigation-data.sql

If a new top-level menu is desired, that can be added first. Create a new constant for the menu name and add it to the list of menu constants that start on line 17. An example would be:

NEW_MENU CONSTANT NUMBER := 21;

Then declare that top-level menu:

insert_navigation_item(NEW_MENU, TOP_LEVEL_MENU, 'New Menu', '', '', STATUS_NORMAL, ORDER_SIXTH, '', ENABLED, IS_NAVIGABLE);

Eitehr way, add a new constant with the name of the page to the list of constants that begin on line 100. It's value will be a number greater than
100. Use the next available number.

Then, make a call to the insert_navigation_item using the appopriate parent page. An example is here:

 insert_navigation_item(MANAGE_EXECUTIVE_HOURS_PAGE, FISCAL_CLOSE, 'Manage Executive Hours', 'PROF-DOLLAR-EXEC-EXTRACT, TPR008-09', 'manage-executive-hours-and-dollars', STATUS_NORMAL, ORDER_FIRST, '', ENABLED, IS_NAVIGABLE);

Note: if the page is not navigable, the last constant value should be: NOT_NAVIGABLE. Also, a page that is not navigable does not need to have a menu parent for the second parameter, it can be TOP_LEVEL_MENU.

Then, add permissions for whatever roles are desired with a call to assign_navigation_role. Here is an example:

assign_navigation_role(MANAGE_EXECUTIVE_HOURS_PAGE, SYSTEM_ADMINISTRATOR);

The last steps are to update two files: 

./src/services/src/Demoulas.ProfitSharing.Data/Entities/Navigations/Navigation.cs
./src/services/tests/Demoulas.ProfitSharing.UnitTests.Common/Fakes/NavigationFaker.cs

In Navigation.cs, using the name of your page, add a constant to the end of the Constants class.

In NavigationFaker.cs, add a new Navigation object to the List that starts on line 49.




