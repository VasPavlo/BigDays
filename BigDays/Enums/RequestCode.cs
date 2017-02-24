using System;
namespace BigDays.Enums
{
	public enum RequestCode
	{
		Edit_BigDay = 10,
		AddNew_BigDay =20,
		List_BigDays = 30,
		PickImage =1000,
		ReturnPickImagePath = 5000,
		CameraImage = 2000,
		Repeat = 3000,
		Alerts =4000,
        PermissionsCameraAndWriteExternalStorage = 44,
        PermissionsWriteExternalStorage=45
    }
}
