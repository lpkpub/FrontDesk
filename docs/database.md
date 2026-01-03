# Database

## Notes
- PostgreSQL database.
- Primary Keys (Id) are Guid.
- EF Core relationships fully-defined with navigation properties.
- All conventional C# Pascal-case naming is converted to snake_case.

## Tables
### User
- Id
- Name *string*
- Pin *string, nullable, index, 6-digit*
- UserType *enum: string conversion*
- IsActive *bool*
- Visits *ICollection, Visit collection navigation*

### Visit
- Id
- EntryAt *DateTime*
- ExitAt *DateTime, nullable*
- UserId *Guid, User foreign key*
- User *User, navigation*
- Company *string, nullable*
- PhoneNumber *string, nullable*
- Reason *string*