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

//==================================================

// introduce new property "Resources" to all documents
```
from "Employees" 
update {
    this.Resources = {
        "key": "value",
        "prop_for_deletion" : {
            "delete": true
        },
        "prop" : {
            "delete": false
        }
    };
}
```

// delete all properties that have "delete" : true
```
from "Employees" 
update {
    Object.keys(this.Resources).forEach(key => {
        if (this.Resources[key].delete === true) {
            delete this.Resources[key];
        }
    });
}
```

