using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ookii.Dialogs.Wpf;

namespace Papyrus.Studio.Framework
{

	public static class MessageBoxEx
	{

		public enum MessageBoxResult
		{

			Invalid,
			Yes,
			No,
			Cancel
		

		}

		public static void ShowInfo(string title, string message, string okText)
		{

			var taskDialog = new TaskDialog();

			taskDialog.WindowTitle = title;

			var yesButton = new TaskDialogButton(ButtonType.Ok);
			yesButton.Default = true;
			yesButton.ButtonType = ButtonType.Ok;

			taskDialog.Buttons.Add(yesButton);
		
			taskDialog.MainInstruction = title;
			taskDialog.Content = message;

			taskDialog.MainIcon = TaskDialogIcon.Information;

			taskDialog.ShowDialog();

		}

		public static MessageBoxResult ShowConfirm(string title, string message, string yesText = "Yes", string noText = "No", string cancelText = null, string extraVerificationText = null)
		{

			var taskDialog = new Ookii.Dialogs.Wpf.TaskDialog();

			taskDialog.WindowTitle = title;

			var yesButton = new TaskDialogButton(yesText);
			yesButton.Default = true;
			yesButton.ButtonType = ButtonType.Yes;

			var noButton = new TaskDialogButton(noText);
			noButton.ButtonType = ButtonType.No;

			taskDialog.Buttons.Add(yesButton);
			taskDialog.Buttons.Add(noButton);

			TaskDialogButton cancelButton = null;

			if (cancelText != null) {
				cancelButton = new TaskDialogButton(cancelText);
				cancelButton.ButtonType = ButtonType.Cancel;
				taskDialog.Buttons.Add(cancelButton);
				taskDialog.AllowDialogCancellation = true;
			} else {
				taskDialog.AllowDialogCancellation = false;
			}

			if (extraVerificationText != null) {

				taskDialog.VerificationText = extraVerificationText;
				yesButton.Enabled = taskDialog.IsVerificationChecked = false;

				taskDialog.VerificationClicked += (sender, args) => {
					yesButton.Enabled = taskDialog.IsVerificationChecked;
				};

			}

			taskDialog.MainInstruction = title;
			taskDialog.Content = message;

			taskDialog.MainIcon = TaskDialogIcon.Warning;

			var result = taskDialog.ShowDialog();

			var ret = MessageBoxResult.Invalid;

			if (result == yesButton)
				ret = MessageBoxResult.Yes;

			if (result == noButton)
				ret = MessageBoxResult.No;

			if(result == cancelButton)
				ret = MessageBoxResult.Cancel;

			return ret;

		}

	}

}
