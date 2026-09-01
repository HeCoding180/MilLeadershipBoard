# MilLeadershipBoard

The MilLeadershipBoard is a military dashboard used to track troop locations, training lessions and daily/weekly schedules during training.

**Note**:
This application is currently in a demo version. It has the basic functionality that were originally planned but not much work has been put into UX *yet*. Many things regarding UX and UI design might change in the future.

## Dashboard View

The following image shows the main dashboard view:

![Screenshot of the dashboard view](/docs/Images/Dashboard_View.png)

**Note**: The names used in the above image are **not real names** and were generated using ChatGPT! They are thus in no way linked to any real troops!

### Locations Tile

The locations tile is used to track troop locations if the platoon is split up. It contains the following features:
- New tiles can be added with the plus button on the top right of the locations tile.
- Existing tiles can be renamed or deleted using the dropdown buttons of the individual tiles.
- The dropdown also contains a "Make default" button which is used to make a location the default location. The default location defines where new soldiers that are created in the [table view](#soldiers-table-view) are added and also generally acts as a fallback location if other locations are deleted as well as for upcomming features like automatic assignment for planned special assignments.

*Note*: This tile is in a demo state. Larger changes are likely to occur.

### Weekly Schedule Tile

The weekly schedule tile contains two fields where images of two weekly schedules can be added.

*Note*: This tile is in a demo state. Larger changes are likely to occur.

### Special Assignments Tile

The special assignments tile currently consists of a simple plain text field but will be upgraded in the future to allow for plannable assignments. These plannable assignments will then automatically re-assign the soldiers of that assignment to the specified location and will assign them back once the assignment is complete which can be automatic or manual.

*Note*: This tile is in a demo state. Larger changes are likely to occur.

### Lessions Tile

The lessions tile contains functionality to add and view upcomming lessions together with who is responsible for that lession. Existing lessions can be reordered manually as of now, but this will likely change as soon as lession date and time will be implemented to further automate this too. For now lessions also have to be completed manually.

*Note*: This tile is in a demo state. Larger changes are likely to occur.

### Breaks Tile

The breaks tile is used to track how many breaks have been done over the day. Time can simply be added by pressing one of the pre-defined break duration buttons and can be reset using the cross button.

### Daily Schedules Tile

The daily schedules tile contains images for daily schedules. These schedules have a specified date for which they are for and are automatically removed again once that day has passed.

*Note*: This tile is in a demo state. Larger changes are likely to occur.

## Soldiers Table View

The soldiers table view can be opened by clicking on the three bars on the top left corner of the window and selecting the "Soldiers" tab. It offers a table style view of all soldiers and their location. This might get more information in the future. New soldiers can be added and names, ranks, et cetera can be edited here.

![Screenshot of the table view](/docs/Images/Table_View.png)

**Note**: The names used in the above image are **not real names** and were generated using ChatGPT! They are thus in no way linked to any real troops!