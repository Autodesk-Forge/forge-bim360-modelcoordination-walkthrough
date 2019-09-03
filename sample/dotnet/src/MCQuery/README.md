# MCQuery

The MCQuery application can be used to interactively run model set version index queries and manipulate their results. It also provides a mechanism for downloading model set version JSON, model set version clash results and model derivatives.

```
------------------------------------
    Model Coordination Query API
------------------------------------

   1. Search index fields
   2. Run index query
   3. Print last index query to console
   4. Pretty print last index query to console
   5. Export last index query to CSV
   6. Save index manifest for query to file
   7. Display last query details
   8. Export last index query to NDJSON

   9. Download model derivative manifest
  10. Download model derivative

  11. Download model set version
  12. Download clash test resources

  13. Exit

Select :                                                        
```

## Sample Queries

Count the number of rows in the index.

```sql
select count(*) as row_count from s3object
```

Get the first 500 objects in the index

```sql
select s.* from s3object s limit 500
```

Get the first 50 rows which have a name.

```sql
select * from s3object s where s.p153cb174 is not missing limit 50
```

Get the first 50 rows which have a Revit category, family and type.

```sql
select * from s3object s where s.p20d8441e is not missing and s.p30db51f9 is not missing and s.p13b6b3a0 is not missing limit 50
```

Get the first 50 rows which have a Revit category, family and type and are viewable in more than one view in the original seed file.

```sql
select * from s3object s where s.p20d8441e is not missing and s.p30db51f9 is not missing and s.p13b6b3a0 is not missing and count(s.docs) > 1 limit 50
```