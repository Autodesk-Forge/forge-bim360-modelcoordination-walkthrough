# MCQuery

## Sample Queries

Count the number of rows in the index.

```sql
select count(*) as row_count from s3object
```

Get the first 500 objects in the index

```sql
select s.* from s3object s limit 500
```

Get the file, manifest database, object ID and name for every row which has a name.

```sql
select s.file,
       s.db,
       s.id,
       s.p153cb174
from s3object s
where s.p153cb174 != ''
```

Get the file, manifest database, object ID, name and Revit category, family and type for every row which has a category, family and type defined.

```sql
select s.file,
       s.db, 
       s.id,
       s.p153cb174,
       s.p20d8441e,
       s.p30db51f9,
       s.p13b6b3a0
from s3object s
where s.p20d8441e != ''
and  s.p30db51f9 != ''
and s.p13b6b3a0 != ''
```

Get the file, manifest database, object ID, `__viewable_in__`,  name and Revit category, family and type for every row which has a category, family and type defined.

```sql
select s.file,
       s.db,
       s.id,
       s.pc96e2dfc,
       s.p153cb174,
       s.p20d8441e,
       s.p30db51f9,
       s.p13b6b3a0
from s3object s
where s.p20d8441e != ''
and  s.p30db51f9 != ''
and s.p13b6b3a0 != ''
```