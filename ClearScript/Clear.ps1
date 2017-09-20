
#set execution policy so I can relaunch as admin
Set-ExecutionPolicy -Scope CurrentUser Unrestricted

# set window size -------------------------------------
$h = get-host
$win = $h.ui.RawUI.WindowSize
$win.Width = 62    # set window size
$win.Height = 15
$h.UI.RawUI.set_windowsize($win)

#---------------------------------------------
# create main menu
do {
clear
Write-Host "What do you want to clear?"
Write-Host -foregroundcolor Yellow "-Note that options 3 & 4 require admin permissions-"
write-host " "
  [int]$userMenuChoice = 0
  while ( $userMenuChoice -lt 1 -or $userMenuChoice -gt 5) {
    clear
    Write-Host "1. Clear IE"
    Write-Host "2. Clear Credential Manager"
    Write-Host -ForegroundColor Yellow "---Requires Admin Permissions-------"
    Write-Host "3. Clear CCM Cache"   
    write-host "4. Clear Temp Files"
    Write-Host -foregroundcolor Yellow "------------------------------------"
    Write-Host "5. Quit and Exit"

    [int]$userMenuChoice = Read-Host "Please choose an option"

    switch ($userMenuChoice) {
      1{#-option 1--------------------------------------------------------------------
        clear
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host -ForegroundColor green "Clear IE Data"
        Write-Host -ForegroundColor green "Script for clearing IE data."
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host " "
        Write-Host -ForegroundColor green "Please select the IE data you would like to delete."

        #---------------------------------------------
        $options = [System.Management.Automation.Host.ChoiceDescription[]] @("&TempIEFiles", "&Cookies", "&History", "&FormData", "&Passwords", "A&ddOnSettings", "&All", "Cac&he and Cookies", "&Quit")
        [int]$defaultchoice = 7
        $opt = $host.UI.PromptForChoice($Title , $Info, $Options, $defaultchoice)
        switch($opt)
          {
            0 { Write-Host "Clearing Temp Files" -ForegroundColor Green; Rundll32.exe inetcpl.cpl, ClearMyTracksByProcess 8}
            1 { Write-Host "Clearing Cookies" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 2}
            2 { Write-Host "Clearing History" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 1}
            3 { Write-Host "Clearing Form Data" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 16}
            4 { Write-Host "Clearing Passwords" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 32}
            5 { Write-Host "Clearing Add-On Settings" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 4351}
            6 { Write-Host "Clearing All IE Data!" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 255}
            7 { Write-Host "Clearing Cache & Cookies!" -ForegroundColor Green; RunDll32.exe InetCpl.cpl, ClearMyTracksByProcess 2; Rundll32.exe inetcpl.cpl, ClearMyTracksByProcess 8}
            8 { Write-Host "Operation canceled" -ForegroundColor Yellow}
          }

        #--------------------------------------------- 
        
      }

      2{#-option 2--------------------------------------------------------------------
        clear
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host -ForegroundColor green "Clear Credentials"
        Write-Host -ForegroundColor green "Script for clearing saved credentials."
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host " "

        Write-Host -ForegroundColor green "Are you sure you want to delete all saved Credentials?"

        #---------------------------------------------
        $options = [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes", "&No")
        [int]$defaultchoice = 0
        $opt = $host.UI.PromptForChoice($Title , $Info, $Options, $defaultchoice)
        switch($opt)
          {
            0 {
               # Grab all the files you need to delete
               cmdkey /list | ForEach-Object{if($_ -like "*Target:*"){cmdkey /del:($_ -replace " ","" -replace "Target:","")}}
               Write-Progress -Activity "Clearing Credentials" -Completed
               Pause
              }

            1 { Write-Host "Goodbye" -ForegroundColor Green}
          }


      }














      
      #--Run as ADMIN-------------------------------------------------------------------------------

      3{#-option 2--------------------------------------------------------------------

      
       If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))

            {   
                $arguments = "& '" + $myinvocation.mycommand.definition + "'"
                Start-Process powershell -Verb runAs -ArgumentList $arguments
                
                exit
            }
        # Run your code that needs to be elevated here:

       clear
        #---------------------------------------------
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host -ForegroundColor green "Clear CCMCache"
        Write-Host -ForegroundColor green "Script for clearing ccmcache."
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host " "
        Write-Host -ForegroundColor green "Are you sure you want to delete all data in the CCMCache?"

        #---------------------------------------------
        $options = [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes", "&No")
        [int]$defaultchoice = 0
        $opt = $host.UI.PromptForChoice($Title , $Info, $Options, $defaultchoice)
        switch($opt)
          {
            0 {

              # Write-Host "Clearing CCMCache" -ForegroundColor Green; Remove-Item C:\Windows\ccmcache\* -Recurse -ErrorAction SilentlyContinue

               # Grab all the files you need to delete
                $filesToDelete = Get-ChildItem C:\Windows\ccmcache -Recurse

                for($i = 0; $i -lt $filesToDelete.Count; $i++){
                    # calculate progress percentage
                    $percentage = ($i + 1) / $filesToDelete.Count * 100
                    Write-Progress -Activity "Deleting Files" -Status "Deleting File #$($i+1)/$($filesToDelete.Count)" -PercentComplete $percentage
                    # delete file
                    $filesToDelete[$i] |Remove-Item -Recurse -ErrorAction SilentlyContinue
                }
                # All done
                Write-Progress -Activity "Deleting Files" -Completed
                Pause
              }
            1 { Write-Host "Goodbye" -ForegroundColor Green}
          }
        }

      #--------------------------------------------------------------------------------

      
      4{#-option 3-----------------"C:\Windows\Temp\*" delete-----"C:\Users\421067\AppData\Local\Temp\*" Delete--------
       
       If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))

            {   
                $arguments = "& '" + $myinvocation.mycommand.definition + "'"
                Start-Process powershell -Verb runAs -ArgumentList $arguments
                
                exit
            }
 
        # Run your code that needs to be elevated here:
       
        clear
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host -ForegroundColor green "Clear Temp files"
        Write-Host -ForegroundColor green "Script for clearing temp files."
        Write-Host -ForegroundColor green "This script will clear Windows and User temp files."
        Write-Host -ForegroundColor yellow "##############################################################"
        Write-Host " "
        Write-Host -ForegroundColor green "Are you sure you want to delete all data in temp folders?"

        #---------------------------------------------
        $options = [System.Management.Automation.Host.ChoiceDescription[]] @("&Yes", "&No")
        [int]$defaultchoice = 0
        $opt = $host.UI.PromptForChoice($Title , $Info, $Options, $defaultchoice)
        switch($opt)
          {
            0 {
            
               # Write-Host "Clearing temp files" -ForegroundColor Green; Remove-Item C:\Windows\temp\* -Recurse -ErrorAction SilentlyContinue; Remove-Item C:\Users\*\AppData\Local\Temp\* -Recurse -ErrorAction SilentlyContinue
             
               # Grab all the files you need to delete
               $filesToDelete = Get-ChildItem C:\Windows\temp -Recurse -Force
               $filesToDelete1 = Get-ChildItem C:\Users\*\AppData\Local\Temp -Recurse -Force -ErrorAction SilentlyContinue

               if (!(($filesToDelete.Count -eq 0)))
                 {
                  for($i = 0; $i -lt $filesToDelete.Count; $i++){
                      # calculate progress percentage
                      $percentage = ($i + 1) / $filesToDelete.Count * 100
                      Write-Progress -Activity "Deleting Files" -Status "Deleting File #$($i+1)/$($filesToDelete.Count)" -PercentComplete $percentage
                      # delete file
                      
                      if (!($i -eq 0)){$filesToDelete[$i] |Remove-Item -Recurse -Force -ErrorAction SilentlyContinue}
                      }
                  }

                  if (!(($filesToDelete1.Count -eq 0)))
                 {
                  for($j = 0; $j -lt $filesToDelete1.Count; $j++){
                      # calculate progress percentage
                      $percentage = ($j + 1) / $filesToDelete1.Count * 100
                      Write-Progress -Activity "Deleting Files" -Status "Deleting File #$($j+1)/$($filesToDelete1.Count)" -PercentComplete $percentage
                      # delete file
                      
                      if (!($j -eq 0)){$filesToDelete1[$j] |Remove-Item -Recurse -Force -ErrorAction SilentlyContinue}
                      }
                  }

                # All done
                Write-Progress -Activity "Deleting Files" -Completed
                Pause
             }
            1 { Write-Host "Goodbye" -ForegroundColor Green}
          }
      } 

      
      #- option 5 ----------------------------------------------------------------------------------


      default {
                Write-Host -foregroundcolor Green "Nothing selected, exiting."
                Set-ExecutionPolicy -Scope CurrentUser Restricted
              } 

       
    }
  }
} while ( $userMenuChoice -ne 5 )

#------------------------------------------------------------------------------

