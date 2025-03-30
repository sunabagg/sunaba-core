using Godot;
using System;
using System.Collections.Generic;

namespace Sunaba.Core;

public partial class TabManager : TabContainer
{
	public TabBar TabBar;

	public List<String> TabsWithNoCloseButton = new List<String> { "Game", "Project Browser" };

	public bool AutoCloseTabs = true;

	[Signal]
	public delegate void TabCloseRequestedEventHandler(long idx);

	[Export]
	public bool ShowCloseButton = true;
	public override void _Ready()
	{
		TabBar = GetTabBar();
		TabBar.TabClosePressed += RemoveTab;
	}

	public override void _Process(double delta)
	{
		if (ShowCloseButton == true)
			TabBar.TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowActiveOnly;
		else
			TabBar.TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowNever;
		foreach (var tabName in TabsWithNoCloseButton)
		{
			if (GetNodeOrNull(tabName) != null)
			{
				if (GetTabTitle(CurrentTab) == tabName)
				{
					TabBar.TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowNever;
				}
			}
		}
	}

	public void RemoveTab(long idx)
	{
		EmitSignal(SignalName.TabCloseRequested, idx);
		if (!AutoCloseTabs)
		{
			return;
		}
		var tab = GetTabControl((int)idx);
		if (tab is UiDocument uiDocument)
		{
			uiDocument.DisposeDocument();
		}
		tab.QueueFree();
	}

	public void CloseAllTabs()
	{
		foreach (Node node in GetChildren())
		{
			if (node != TabBar)
			{
				if (node is UiDocument uiDocument)
				{
					uiDocument.DisposeDocument();
				}
				node.QueueFree();
			}
		}
	}

	public void DisposeManager()
	{
		CloseAllTabs();
		QueueFree();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		DisposeManager();
	}

	public void SetCurrentTab(Control control)
	{
		int idx = GetTabIdxFromControl(control);
		CurrentTab = idx;
	}

	public bool IsCurrentTab(Control control)
	{
		int idx = GetTabIdxFromControl(control);
		if (CurrentTab == idx)
		{
			return true;
		}
		else return false;
	}

	public void SetTabIcon(Control control, Texture2D icon)
	{
		int idx = GetTabIdxFromControl(control);
		TabBar.SetTabIcon(idx, icon);
	}

	public void SetTabIconByName(String tabName, Texture2D icon)
	{
		foreach (Node node in GetChildren())
		{
			if (node is Control control)
			{
				if (GetTabTitle(GetTabIdxFromControl(control)) == tabName)
				{
					SetTabIcon(control, icon);
				}
			}
		}
	}

	public Control GetCurrentTabControl()
	{
		var c = GetTabControl(CurrentTab);
		if (c is Control control)
		{
			return control;
		}
		return null;
	}

	public void AddUncloseableTab(String tabName)
	{
		TabsWithNoCloseButton.Add(tabName);
	}
}
