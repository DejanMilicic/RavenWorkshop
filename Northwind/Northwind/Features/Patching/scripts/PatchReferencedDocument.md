```
from Orders update {
    let company = load(this.Company);
    company.BigShipments = this.Freight > 100
    put(id(company), company)
}
```