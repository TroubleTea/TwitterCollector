$accounts = Get-Content .\accounts.json | ConvertFrom-Json

$values = ""
$nl = [Environment]::NewLine
foreach($account in $accounts) {
    $id = $account.rest_id
    $name = $account.name
    $handle = $account.screen_name

    $values += "($id,'$name','$handle'),$nl"
}

$statement = "
insert into user(
    id,
    [name],
    twitter_handle
) values
$values
"
$statement | Out-File users.sql