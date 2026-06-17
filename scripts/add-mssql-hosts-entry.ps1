# Run as Administrator. Points the MSSQL container name at localhost so host
# tools (SSMS, sqlcmd) can connect via doorsweb.mssql,<published-port>.
# In-container clients don't need this; Docker's embedded DNS already resolves it.
$h="$env:windir\System32\drivers\etc\hosts"; if(-not(Select-String -Path $h -Pattern '\bdoorsweb\.mssql\b' -Quiet)){Add-Content -Path $h -Value "`n127.0.0.1`tdoorsweb.mssql"}
