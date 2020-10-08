$connectTestResult = Test-NetConnection -ComputerName stadmin001.file.core.windows.net -Port 445
if ($connectTestResult.TcpTestSucceeded) {
    # Save the password so the drive will persist on reboot
    cmd.exe /C "cmdkey /add:`"stadmin001.file.core.windows.net`" /user:`"Azure\stadmin001`" /pass:`"/BHrhdOfCEcHAeOlyfWfM3fmGzFMQa3EswQzCmvLeZHjx33fBpIvhBOg9SMvNyBrd8POugzlUBFUUZUmJvw8Wg==`""
    # Mount the drive
    New-PSDrive -Name Z -PSProvider FileSystem -Root "\\stadmin001.file.core.windows.net\cloudshell" -Persist
} else {
    Write-Error -Message "Unable to reach the Azure storage account via port 445. Check to make sure your organization or ISP is not blocking port 445, or use Azure P2S VPN, Azure S2S VPN, or Express Route to tunnel SMB traffic over a different port."
}