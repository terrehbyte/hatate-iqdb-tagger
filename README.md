# Hatate
Search and tag images using IQDB.

## Dependencies
* **IqdbApi** by _ImoutoChan_:
[GitHub](https://github.com/ImoutoChan/IqdbApi)
* **dcsoup** by _matarillo_:
[GitHub](https://github.com/matarillo/dcsoup)

## How to use

1. Start the program and import images by drag and drop or by clicking "Add files" or "Add folder" in the "Files" menu
2. Click the "Start" button
3. Leave it open until all the images were searched

Found images will by colored in green in the list or in red otherwise.

When an image is selected in the list the found tags will appear on the right.

When you want to write the tags for the found images, select all the green rows then right click and choose "Write tags".
Files will be moved in the "imgs/tagged/" folder from the app directory.

## Options

The "Options" button in the menubar allows to change some parameters:

### Add rating to tags:

If available the rating will be added as a tag, for example `rating:safe`.

### Minimum match type

If checked, only the results with a match type greater or equal that the selected one will be kept.
For the best results, check this option and select "Best" in the list.

### Minimum number of tags

Define how many tags are needed to keep a result. With a value of `1`, results without any tags will be scrapped.

### Minimum similarity

Define the minimum similarity value to keep a result. A lower value will reduce the accuracy but don't use a value too high as no search will produce a 100% match.

### Delay

To prevent from abusing the IQDB service and being banned from it, a certain wait time is applied between each search.
The default is 60 seconds, consider increasing it if you have a lot of time fot it to run but don't reduce it to much.

### Randomize the delay

Will use a random delay based on the one from the previous option.

### Automatically move files when no user action is needed

Not found results or found without unknown tags will be moved automatically after being searched.

### Ask for tags when adding files to the list

When importing files into the program a window will be shown asking for tags
Those tags will be added to the known tag list for each imported files.

### Always rename move files using MD5

Usually when a file is moved to the `tagged` or `not found` folder it will keep its original name unless the resulting path is too long or a file with the same name already exists in the destination folder, in that case the file will be renamed by calculating its MD5.
If this option is checked, all the moved files will be renamed taht way.

### Sources

Only the results from the checked sources will be kept when searching.
If an image only have results from unchecked sources it will be marked as not found.
