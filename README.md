# Plugins for Batch Processing Files in Revit

<a href="README.ru-RU.md">Русский</a> | <a href="README.md">English</a>

***Supports versions 2019-2026***

  **The plugin includes 7 modules:**
  - 6 for export
  - 1 for import

 ### Export:
  - [Export NWC (Navisworks Cache)](#export-nwc)
  - [Export IFC (Industry Foundation Classes)](#export-ifc)
  - [Export Detached Models](#export-detached-models)
  - [Transmit Models (***May be deprecated in the future***)](#transmit-models)
  - [Migrate Models](#migrate-models)
  - [Export Parameters](#export-parameters)

### Import:
  - [Import RVT](#import-rvt)

## Export NWC
This module allows batch exporting models to Navisworks cache (.nwc) with predefined settings.

  **Key features:**
  - Export configurations can be saved to a .json file for reuse.
  - Multiple configurations can be added to a list to export multiple batches. *(Different objects, settings for various purposes, etc.)*

![nwc](https://github.com/user-attachments/assets/8a78ce83-3c0b-4ac0-9242-01cb7607a750)

## Export IFC
This module allows batch exporting models to .ifc files with predefined settings.

 **Key features:**
  - Export configurations can be saved to a .json file for reuse.

![ifc](https://github.com/user-attachments/assets/4fc70ff8-7b1b-4d70-8e90-ede45c8cc59c)

## Export Detached Models
This module allows batch exporting models for sharing outside the organization.<br>
It implements functionality similar to *eTransmit* but works faster by skipping nested link searches.<br>
It is also useful for quickly preparing models received from contractors.

  **Key features:**
  - The model list can be saved to a text file for reuse.
  - Files can be relocated using masks. *(e.g., preserving folder structures.)*
  - Files can be automatically renamed using masks.
  - Files are automatically upgraded to the active Revit version.
  - All links can be removed from the model.
  - Models can be checked for "empty views" to filter out empty files from third parties.
  - Models can be purged of empty worksets. *(Available from version **2022**)*
  - Models can be purged of all unused elements. *(Available from version **2024**)*
  
![detach](https://github.com/user-attachments/assets/fc7e8ebe-9ac9-4068-83c8-ff91a0c64e9c)

## Transmit Models
***This module may be deprecated as its functionality overlaps with the Detach module.*** <br>

This module exports batches of models for sharing outside the organization. <br>
It mimics *eTransmit* but skips nested link searches for faster performance. <br>

*Technically, the plugin copies files and sets the **IsTransmitted = true** flag. Its primary purpose is to quickly share models without triggering errors on opening. For cleaning or upgrading models, use the Detach module instead.*

## Migrate Models
This module migrates projects to new locations while preserving link structures. For example, moving from design stage "P" to stage "R". <br>
It uses a .json file containing *Dictionary<string, string>*, where the key is the source file path and the value is the target path.

## Export parameters
This module exports defined parameters from provided models to single CSV file.

![params](https://github.com/user-attachments/assets/dbcdfd0d-9e75-4925-acb3-c456b5f4592e)

## Import RVT
This module allows batch importing multiple RVT links in a single action.
