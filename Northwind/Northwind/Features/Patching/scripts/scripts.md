// for all documents that do not have "Age" property
// create it and initialize to 0
```
from "Employees" 
where true and not exists("Age")
update {
    this.Age = 0;
}
```

// for all documents that have
// "Age" : null
// set it to 55
```
from "Employees" 
where Age = null
update {
    this.Age = 55;
}
```

// for all documents that do not have "Resources" property
// add new JSON object to it
```
from "Employees" 
where true and not exists("Resources")
update {
    this.Resources = {
        "key": "value",
        "prop" : {
            "k1": "v1",
            "k2": "v2"
        }
    };
}
```