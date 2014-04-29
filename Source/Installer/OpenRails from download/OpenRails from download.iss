; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FROM INNO SETUP FOR DETAILS ON CREATING SCRIPT FILES.

#define NetRedistPath "..\..\..\Microsoft .NET Framework Redistributable 3.5 SP1 download manager"
#define NetRedist "dotnetfx35setup.exe"
#define OutputBaseFilename "setup_OR_pre-v1.0_from_download"

#include "..\OpenRails shared\OpenRails.iss"

procedure InstallFrameworkNet35SP1;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := 'Installing Framework .NET v3.5 SP1 (downloads 240MB and takes about 10 mins) ...';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    begin
      // Install the package
      if not Exec(ExpandConstant('{tmp}\{#NetRedist}'), ' /q /noreboot', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
      begin
        // Tell the user why the installation failed
        MsgBox('Installing Framework .NET v3.5 SP1 failed with code: ' + IntToStr(ResultCode) + '.', mbError, MB_OK);
      end;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
  end;
end;

