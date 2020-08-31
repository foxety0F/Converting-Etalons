Sber Testing - Prepare Etalons

Author of this application :
* foxety0f

Releases
Version 0.1
1. Formatting the source xls * file to the required format for testing, namely :
* Replace in columns-row all #(sharp) to "_". For example testing#name replace to testing_name;
* Modify column name if in name exist "." (dot). For example : testing.name replace to name;
* Modify column name and change case to lowercase. For example : NAME to name;
* Modify new-line character to UNIX-format(\n);
* Modify name of xls* from name with "."(dot). For example : test_schema.test to test;
* Save target file with UTF-8 encoding;
2. Save file into directory : 
* Save converted csv file as is
* Save xls* files from directory to converted csv files as is
* Save xls* files from directory to converted csv files to RAR-archive
* Column separator on default - ; , may be change
* Row separator on default - " , may be change
* Generating lotrace-file into target directory

Publishing by Apache License 2.0