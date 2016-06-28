﻿using System.Linq;

namespace SIM.Pipelines.Install
{
  #region

  using Microsoft.Web.Administration;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  [UsedImplicitly]
  public class SetupWebsite : InstallProcessor
  {
    #region Methods

    #region Protected methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var name = args.Name;
      var webRootPath = args.WebRootPath;
      var enable32BitAppOnWin64 = args.Is32Bit;
      var forceNetFramework4 = args.ForceNetFramework4;
      var isClassic = args.IsClassic;
      var bindingInfos = args.HostNames.Select(hostName => new BindingInfo("http", hostName, 80, "*")).ToArray();
      var id = SetupWebsiteHelper.SetupWebsite(enable32BitAppOnWin64, webRootPath, forceNetFramework4, isClassic, bindingInfos, name);
      args.Instance = InstanceManager.GetInstance(id);
    }

    #endregion

    #region Private methods

    [NotNull]
    private static string ChooseAppPoolName([NotNull] string name, [NotNull] ApplicationPoolCollection applicationPools)
    {
      Assert.ArgumentNotNull(name, "name");
      Assert.ArgumentNotNull(applicationPools, "applicationPools");

      var modifier = 0;
      var newname = name;
      while (applicationPools[newname] != null)
      {
        newname = name + '_' + ++modifier;
      }

      return newname;
    }

    private ProcessModelIdentityType GetIdentityType([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      if (name.EqualsIgnoreCase("NetworkService"))
      {
        return ProcessModelIdentityType.NetworkService;
      }

      if (name.EqualsIgnoreCase("ApplicationPoolIdentity"))
      {
        return ProcessModelIdentityType.ApplicationPoolIdentity;
      }

      if (name.EqualsIgnoreCase("LocalService"))
      {
        return ProcessModelIdentityType.LocalService;
      }

      if (name.EqualsIgnoreCase("LocalSystem"))
      {
        return ProcessModelIdentityType.LocalSystem;
      }

      return ProcessModelIdentityType.SpecificUser;
    }

    #endregion

    #endregion
  }
}