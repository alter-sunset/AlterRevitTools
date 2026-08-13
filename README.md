# Plugins for Batch Processing Files in Revit

<a href="README.ru-RU.md">Русский</a> | <a href="README.md">English</a>

***Supports versions 2019-2027***

  **The plugin includes 6 modules:**
  - 3 for export
  - 1 for import
  - 2 tools

 ### Export:
  - [Export Models](#export-models)
  - [Migrate Models](#migrate-models)
  - [Export Parameters](#export-parameters)

### Import:
  - [Import RVT](#import-rvt)

### Tools:
  - [Purge](#purge)
  - [Delink](#delink)

## Export Models
This module allows batch exporting models:
  - To Navisworks cache (.nwc) with predefined settings,
  - To .ifc files with predefined settings
  - For sharing outside the organization. (It implements functionality similar to *eTransmit* but works faster by skipping nested link searches. It is also useful for quickly preparing models received from contractors.)

  **Key features:**
  - Export configurations can be saved to a .json file for reuse.
  - Files are automatically upgraded to the active Revit version.
  - All links can be unloaded or removed from the model.
  - Models can be purged of empty worksets. *(Available from version **2022**)*
  - Models can by purged of unassigned rooms.
  - Models can be purged of all unused elements.
  - Models can be cleaned of sheets and views.

<img width="886" height="593" alt="image" src="https://github.com/user-attachments/assets/1145f4f1-c221-457c-a4e2-eef0a0e527af" />
<img width="436" height="623" alt="image" src="https://github.com/user-attachments/assets/16e6aff9-fb44-4c4c-aec0-23abecd6b571" />
<img width="1106" height="553" alt="image" src="https://github.com/user-attachments/assets/04604b98-0983-43c0-aa09-a2289c4c4a13" />
<img width="536" height="393" alt="image" src="https://github.com/user-attachments/assets/f63e1d76-b80e-46f0-a4dd-0d44d2b053f4" />

## Migrate Models
This module migrates projects to new locations while preserving link structures. For example, moving from design stage "P" to stage "R". <br>
It uses a .json file containing *Dictionary<string, string>*, where the key is the source file path and the value is the target path.

## Export parameters
This module exports defined parameters from provided models to single CSV file.

<img width="786" height="493" alt="image" src="https://github.com/user-attachments/assets/31c04398-dd48-44e7-b64a-47591e21c539" />

## Import RVT
This module allows batch importing multiple RVT links in a single action.

<img width="786" height="463" alt="image" src="https://github.com/user-attachments/assets/fec34487-6aea-4c2d-a6ce-d7fe9df2dabc" />

## Purge
Purge all unused elements from current model in one go.

## Delink
Remove all linked documents from current model.
