
Mo website chinh truoc: `https://localhost:44359`

Vao thu muc test: `cd D:\Code\ASP.NET\WebsiteTuyenDung\WebsiteTuyenDung.Tests`

Tat ca test: `dotnet test`

Login: `dotnet test --filter "FullyQualifiedName~LoginTests"`

Dang ky: `dotnet test --filter "FullyQualifiedName~RegisterTests"`

Tim viec: `dotnet test --filter "FullyQualifiedName~JobSearchTests"`

Ho so ung vien: `dotnet test --filter "FullyQualifiedName~CandidateProfileTests"`

CV: `dotnet test --filter "FullyQualifiedName~CvTests"`

Ung tuyen: `dotnet test --filter "FullyQualifiedName~ApplyJobTests"`

Nha tuyen dung: `dotnet test --filter "FullyQualifiedName~EmployerJobTests"`

Admin: `dotnet test --filter "FullyQualifiedName~AdminApprovalTests"`

Unit test logic: `dotnet test --filter "FullyQualifiedName~UnitTests"`


Tat ca test cham: `dotnet test --settings .\slow.runsettings`

Login cham: `dotnet test --settings .\slow.runsettings --filter "FullyQualifiedName~LoginTests"`

Chay nhom bat ky cham: `dotnet test --settings .\slow.runsettings --filter "FullyQualifiedName~TenFileTests"`

Bat debug Inspector: `$env:PWDEBUG="1"`

Chay debug Login: `dotnet test --filter "FullyQualifiedName~LoginTests"`

Tat debug Inspector: `Remove-Item Env:\PWDEBUG -ErrorAction SilentlyContinue`

Mo browser khong debug: `$env:HEADED="1"`

Tat mo browser: `Remove-Item Env:\HEADED -ErrorAction SilentlyContinue`

Thu tu nen chay: UnitTests -> Login -> Register -> JobSearch -> CandidateProfile -> Cv -> EmployerJob -> AdminApproval -> ApplyJob.
