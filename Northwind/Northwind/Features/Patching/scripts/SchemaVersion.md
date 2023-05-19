# set initial schema version
```
from Employees
update {
    if(!this["@metadata"]["schema_version"]) {
        this["@metadata"]["schema_version"] = 1;
    }
}
```

# migrate to next version
```
from Employees
update {
    if(this["@metadata"]["schema_version"] < 2) {
        this.FullName = this.FirstName + " " + this.LastName;
        delete this.FirstName;
        delete this.LastName;
        this["@metadata"]["schema_version"] = 2;
    }
}
```
