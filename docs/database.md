# Database

## Notes
- PostgreSQL database.
- Primary Keys (Id) are Guid.
- EF Core relationships fully-defined with navigation properties.
- All conventional C# Pascal-case naming is converted to snake_case.

## Tables
```csharp

User {
	Guid		Id
	string		Name
	string?		Pin			// index, 6-digit-only
	string		UserType	// converted from UserType enum
	bool		IsActive
	ICollection	Visits		// Collection navigation
}

Visit {
	Guid		Id
	DateTime	EntryAt
	DateTime?	ExitAt
	Guid		UserId		// FK -> User.Id
	User		User		// navigation
	string?		Company
	string?		PhoneNumber
	string		Reason
}

```