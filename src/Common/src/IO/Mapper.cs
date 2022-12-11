using Phanes.Mapper;
using Xml;

namespace Phanes.Common.IO;

public static class Mapper
{
	private const ushort CURRENT_VERSION = 1; // Optimistic :)
	
	public static Map Load(string text)
	{
		XMLDocument doc = XMLDocument.Deserialize(text);
		
		if (!doc.Metadata.Exists("version"))
			throw new IOException("Did not find Map Version");

		return doc.Metadata["version"] switch
		{
			"1" => Load_1(doc.Root),
			_ => throw new IOException("Version not supported"),
		};
	}

	public static XMLDocument Save(Map map)
	{
		return CURRENT_VERSION switch
		{
			1 => Save_1(map),
			_ => throw new IOException("Version not supported"),
		};
	}
	
	private static Map Load_1(XMLNode root)
	{
		return _Load_1.Map(root);
	}

	public static XMLDocument Save_1(Map map)
	{
		XMLNode node = _Save_1.Map(map);

		XMLDocument doc = new(node);
		
		doc.Metadata.Add("version", "1");

		return doc;
	}
}

#region Version 1

file static class _Load_1
{
	private static Map s_map;

	public static Map Map(XMLNode node)
	{
		s_map = new();

		s_map.Colours = new(Colours(node.Children["Colours"]));
		s_map.Symbols = new(Symbols(node.Children["Symbols"]));
		s_map.Instances = new(Instances(node.Children["Instances"]));

		s_map.Title = node.Attributes["title"];

		return s_map;
	}
	
	#region Colours
	
	public static IEnumerable<Colour> Colours(XMLNode node)
	{
		return node.Children.Select(Colour);
	}

	public static Colour Colour(XMLNode node)
	{
		Guid id = Guid.Parse(node.Attributes["id"]);

		string name = node.Attributes["name"];

		uint colour = uint.Parse(node.Attributes["hex"]);

		return new(id, name, colour);
	}
	
	#endregion
	
	#region Symbols

	public static IEnumerable<Symbol> Symbols(XMLNode node)
	{
		return node.Children.Select(Symbol);
	}
	
	public static Symbol Symbol(XMLNode node)
	{
		return node.Name switch
		{
			"PointSymbol" => PointSymbol(node),
			"LineSymbol" => LineSymbol(node),
			"AreaSymbol" => AreaSymbol(node),
			"TextSymbol" => TextSymbol(node),
			_ => throw new InvalidOperationException("Invalid symbol type"),
		};
	}

	public static (Guid id, string name, string desc, SymbolNumber num, bool uncr, bool help) SymbolBase(XMLNode node)
	{
		Guid id = Guid.Parse(node.Attributes["id"]);
		string name = node.Attributes["name"];
		string desc = node.Attributes["description"];
		SymbolNumber num = new(node.Attributes["number"]);
		
		bool isUncrossable = bool.Parse(node.Attributes["isUncrossable"]),
			 isHelper = bool.Parse(node.Attributes["isHelper"]);

		return (id, name, desc, num, isUncrossable, isHelper);
	}
	
	public static PointSymbol PointSymbol(XMLNode node)
	{
		var bas = SymbolBase(node);
		
		IEnumerable<MapObject> mapObjects = MapObjects(node.Children["MapObjects"]);
		bool isLocked = bool.Parse(node.Attributes["isLocked"]);	

		return new(s_map, bas.id, bas.name, bas.desc, bas.num, bas.uncr, bas.help, mapObjects, isLocked);
	}

	public static LineSymbol LineSymbol(XMLNode node)
	{
		var bas = SymbolBase(node);
		
		XMLNode style = node.Children["Style"];

		Guid colId = Guid.Parse(style.Children["Colour"].InnerText);
		float width = float.Parse(style.Children["Width"].InnerText);
		
		DashStyle dash = DashStyle.None;

		if (style.Children.Exists("DashStyle"))
		{
			XMLAttributeCollection d = style.Children["DashStyle"].Attributes;

			dash = new(
				float.Parse(d["dashLength"]),
				float.Parse(d["gapLength"]),
				int.Parse(d["groupSize"]),
				float.Parse(d["groupGapLength"])
				);
		}

		MidStyle mid = MidStyle.None;

		if (style.Children.Exists("MidStyle"))
		{
			XMLNode m = style.Children["MidStyle"];

			List<MapObject> objs = new(); // TODO: Load map objects

			mid = new(
				objs,
				float.Parse(m.Attributes["gapLength"]),
				bool.Parse(m.Attributes["requireMid"]),
				float.Parse(m.Attributes["initialOffset"]),
				float.Parse(m.Attributes["endOffset"])
				);
		}
		
		return new LineSymbol(s_map, bas.id, bas.name, bas.desc, bas.num, bas.uncr, bas.help, s_map.Colours[colId], width, dash, mid);
	}

	public static AreaSymbol AreaSymbol(XMLNode node)
	{
		var bas = SymbolBase(node);

		XMLNode style = node.Children["Style"];
		XMLNode fillNode = style.Children[0];

		IFill fill = fillNode.Name switch
		{
			"SolidFill" => new SolidFill(s_map.Colours[Guid.Parse(fillNode.InnerText)]),
			_ => throw new NotImplementedException()
		};

		XMLNode border = style.Children["Border"];

		Guid colId = Guid.Parse(border.Children["Colour"].InnerText);
		float width = float.Parse(border.Children["Width"].InnerText);

		DashStyle dash = DashStyle.None;

		if (style.Children.Exists("DashStyle"))
		{
			XMLAttributeCollection d = style.Children["DashStyle"].Attributes;

			dash = new(
				float.Parse(d["dashLength"]),
				float.Parse(d["gapLength"]),
				int.Parse(d["groupSize"]),
				float.Parse(d["groupGapLength"])
				);
		}

		MidStyle mid = MidStyle.None;

		if (style.Children.Exists("MidStyle"))
		{
			XMLNode m = style.Children["MidStyle"];

			IEnumerable<MapObject>objs = MapObjects(m.Children["MapObjects"]);

			mid = new(
				objs,
				float.Parse(m.Attributes["gapLength"]),
				bool.Parse(m.Attributes["requireMid"]),
				float.Parse(m.Attributes["initialOffset"]),
				float.Parse(m.Attributes["endOffset"])
				);
		}
		
		return new AreaSymbol(s_map, bas.id, bas.name, bas.desc, bas.num, bas.uncr, bas.help, fill, s_map.Colours[colId], width, dash, mid);
	}

	public static TextSymbol TextSymbol(XMLNode node)
	{
		throw new NotImplementedException();
	}
	
	#endregion
	
	#region Map Objects

	public static IEnumerable<MapObject> MapObjects(XMLNode node)
	{
		return node.Children.Select(MapObject);
	}

	public static MapObject MapObject(XMLNode node)
	{
		Guid id = Guid.Parse(node.Attributes["id"]);

		switch (node.Name)
		{
			case "PointObject": {
				XMLNode inner = node.Children["Style"].Children["Inner"];
				XMLNode outer = node.Children["Style"].Children["Outer"];
				
				Colour iCol = inner.Children["Colour"].InnerText == "Transparent"
					? Phanes.Mapper.Colour.Transparent : s_map.Colours[Guid.Parse(inner.Children["Colour"].InnerText)];
				
				Colour oCol = outer.Children["Colour"].InnerText == "Transparent"
					? Phanes.Mapper.Colour.Transparent : s_map.Colours[Guid.Parse(outer.Children["Colour"].InnerText)];
				
				float iWidth = float.Parse(inner.Children["Width"].InnerText);
				float oWidth = float.Parse(outer.Children["Width"].InnerText);

				return new PointObject(s_map, id, iCol, oCol, iWidth, oWidth);
			}
			case "LineObject": {
				XMLNode style = node.Children["Style"];

				Colour col = style.Children["Colour"].InnerText == "Transparent"
					? Phanes.Mapper.Colour.Transparent : s_map.Colours[Guid.Parse(style.Children["Colour"].InnerText)];

				float width = float.Parse(style.Children["Width"].InnerText);

				XMLNode points = node.Children["Points"];
				IEnumerable<vec2> pts = points.Children.Select(
					pt => new vec2(float.Parse(pt.Attributes["x"]), float.Parse(pt.Attributes["y"])));
				
				return new LineObject(s_map, id, pts, width, col);
			}
			case "AreaObject": {
				XMLNode style = node.Children["Style"];
				XMLNode fillNode = style.Children[0]; // Maybe make more robust?
				
				IFill fill = fillNode.Name switch
				{
					"SolidFill" => new SolidFill(s_map.Colours[Guid.Parse(fillNode.InnerText)]),
					_ => throw new NotImplementedException(),
				};
				
				XMLNode border = style.Children["Border"];
				
				Colour col = border.Children["Colour"].InnerText == "Transparent"
					? Phanes.Mapper.Colour.Transparent : s_map.Colours[Guid.Parse(border.Children["Colour"].InnerText)];
				
				float width = float.Parse(border.Children["Width"].InnerText);
				
				XMLNode points = node.Children["Points"];
				IEnumerable<vec2> pts = points.Children.Select(
					pt => new vec2(float.Parse(pt.Attributes["x"]), float.Parse(pt.Attributes["y"])));
				
				return new AreaObject(s_map, id, pts, width, col, fill);
			}
			case "TextObject":
				throw new NotImplementedException();
			default:
				throw new InvalidOperationException("Invalid map object type");
		}
	}
	
	#endregion

	#region Instances

	public static IEnumerable<Instance> Instances(XMLNode node)
	{
		return node.Children.Select(Instance);
	}

	public static Instance Instance(XMLNode node)
	{
		return node.Name switch
		{
			"PointInstance" => PointInstance(node),
			"LineInstance" => PathInstance(node),
			"AreaInstance" => PathInstance(node),
			"TextInstance" => TextInstance(node),
			_ => throw new InvalidOperationException("Invalid instance type")
		};
	}

	public static (Guid id, int layer, Symbol sym) InstanceBase(XMLNode node)
	{
		Guid id = Guid.Parse(node.Attributes["id"]);
		int layer = int.Parse(node.Attributes["layer"]);

		Guid symbolId = Guid.Parse(node.Children["Symbol"].InnerText);
		Symbol symbol = s_map.Symbols[symbolId];

		return (id, layer, symbol);
	}

	public static PointInstance PointInstance(XMLNode node)
	{
		var bas = InstanceBase(node);
		
		vec2 centre = new(
			float.Parse(node.Children["Centre"].Attributes["x"]),
			float.Parse(node.Children["Centre"].Attributes["y"]));

		float rotation = 0;
		if (node.Attributes.Exists("rotation"))
			rotation = float.Parse(node.Attributes["rotation"]);

		return new(bas.id, bas.layer, (PointSymbol)bas.sym, centre, rotation);
	}

	public static PathInstance PathInstance(XMLNode node)
	{
		var bas = InstanceBase(node);
		
		        PathCollection pC = new();
        XMLNode segments = node.Children["Segments"];

        segments.Children.ForEach((seg) =>
        {
            switch (seg.Name)
            {
                case "LinearPath": 
                {
                    List<vec2> pts = new();
                    foreach (XMLNode p in seg.Children)
                    {
                        pts.Add(new(
                                float.Parse(p.Attributes["x"]),
                                float.Parse(p.Attributes["y"])
                                ));
                    }

                    pC.Add(new LinearPath(pts));
                } break;
                case "BezierPath":
                {
                    List<BezierPoint> pts = new();

                    foreach (XMLNode p in seg.Children)
                    {
                        vec2 early = new(
                            float.Parse(p.Children["EarlyControl"].Attributes["x"]),
                            float.Parse(p.Children["EarlyControl"].Attributes["y"])
                            );

                        vec2 anchor = new(
                            float.Parse(p.Children["Anchor"].Attributes["x"]),
                            float.Parse(p.Children["Anchor"].Attributes["y"])
                            );

                        vec2 late = new(
                            float.Parse(p.Children["LateControl"].Attributes["x"]),
                            float.Parse(p.Children["LateControl"].Attributes["y"])
                            );

                        pts.Add(new(anchor, early, late));
                    }

                    pC.Add(new BezierPath(pts));
                } break;
                default: throw new InvalidOperationException();
            }
        });

        return node.Name switch
        {
            "LineInstance" => new LineInstance(bas.Item1, bas.Item2, (LineSymbol)bas.Item3, pC),
            "AreaInstance" => new AreaInstance(bas.Item1, bas.Item2, (AreaSymbol)bas.Item3, pC),
            _ => throw new InvalidOperationException()
        };
	}

	public static TextInstance TextInstance(XMLNode node)
	{
		throw new NotImplementedException();
	}
	
	#endregion
}

file static class _Save_1
{
	public static XMLNode Map(Map map)
	{
		XMLNode node = new("Map");

		node.AddAttribute("title", map.Title);

		node.AddChild(Colours(map.Colours));
		node.AddChild(Symbols(map.Symbols));
		node.AddChild(Instances(map.Instances));

		return node;
	}
	
	#region Colours

	public static XMLNode Colours(IEnumerable<Colour> colours)
	{
		XMLNode node = new("Colours");
		foreach (Colour col in colours)
			node.Children.Add(Colour(col));
		return node;
	}
	
	public static XMLNode Colour(Colour colour)
	{
		XMLNode node = new("Colour");

		node.AddAttribute("id", colour.Id.ToString());
		node.AddAttribute("name", colour.Name);
		node.AddAttribute("hex", colour.HexValue.ToString());

		return node;
	}

	#endregion
	
	#region Fill

	public static XMLNode Fill(IFill fill)
	{
		return fill switch
		{
			SolidFill s => SolidFill(s),
			ObjectFill o => ObjectFill(o),
			_ => throw new InvalidOperationException("Invalid fill type"),
		};
	}

	public static XMLNode SolidFill(SolidFill fill)
	{
		return new("SolidFill")
		{
			InnerText = fill.Colour.Name == "Transparent" ?
				"Transparent" : fill.Colour.Id.ToString(),
		};
	}

	public static XMLNode ObjectFill(ObjectFill fill)
	{
		throw new NotImplementedException();
	}
	
	#endregion
	
	#region Symbols

	public static XMLNode Symbols(IEnumerable<Symbol> symbols)
	{
		XMLNode node = new("Symbols");
		foreach (Symbol sym in symbols)
			node.Children.Add(Symbol(sym));
		return node;
	}
	
	public static XMLNode Symbol(Symbol sym)
	{
		return sym switch
		{
			Phanes.Mapper.PointSymbol p => PointSymbol(p),
			Phanes.Mapper.LineSymbol l => LineSymbol(l),
			Phanes.Mapper.AreaSymbol a => AreaSymbol(a),
			Phanes.Mapper.TextSymbol t => TextSymbol(t),
			_ => throw new InvalidOperationException("Invalid symbol type"),
		};
	}

	public static XMLNode SymbolBase(Symbol sym)
	{
		XMLNode node = new XMLNode("Symbol");
		node.AddAttribute("id", sym.Id.ToString());
		node.AddAttribute("name", sym.Name);
		node.AddAttribute("description", sym.Description);
		node.AddAttribute("number", $"{sym.Number.First}-{sym.Number.Second}-{sym.Number.Third}");
		node.AddAttribute("isUncrossable", sym.IsUncrossable.ToString());
		node.AddAttribute("isHelper", sym.IsHelperSymbol.ToString());

		return node;
	}

	public static XMLNode PointSymbol(PointSymbol sym)
	{
		XMLNode node = SymbolBase(sym);
		node.Name = "PointSymbol";

		XMLNode objs = MapObjects(sym.MapObjects);
		node.AddChild(objs);

		node.AddAttribute("isLocked", sym.IsLockedToNorth.ToString());
		
		return node;
	}

	public static XMLNode LineSymbol(LineSymbol sym)
	{
		XMLNode node = SymbolBase(sym);
		node.Name = "LineSymbol";
		
		XMLNode style = new("Style");
		XMLNode colour = new("Colour");
		XMLNode width = new("Width");
		
		colour.InnerText = sym.Colour.Name == "Transparent"
			? "Transparent" : sym.Colour.Id.ToString();

		width.InnerText = sym.Width.ToString();

		style.AddChild(colour);
		style.AddChild(width);

		if (sym.DashStyle.HasDash)
			style.AddChild(DashStyle(sym.DashStyle));
		if (sym.MidStyle.HasMid)
			style.AddChild(MidStyle(sym.MidStyle));

		node.AddChild(style);

		return node;
	}

	public static XMLNode AreaSymbol(AreaSymbol sym)
	{
		XMLNode node = SymbolBase(sym);
		node.Name = "AreaSymbol";

		XMLNode style = new("Style");
		XMLNode border = new("Border");
		XMLNode borderCol = new("Colour");
		XMLNode width = new("Width");

		XMLNode fill = Fill(sym.Fill);
		
		borderCol.InnerText = sym.BorderColour.Name == "Transparent"
			? "Transparent" : sym.BorderColour.Id.ToString();

		width.InnerText = sym.Width.ToString();

		border.AddChild(borderCol);
		border.AddChild(width);

		style.AddChild(fill);
		style.AddChild(border);
		
		if (sym.DashStyle.HasDash)
			style.AddChild(DashStyle(sym.DashStyle));
		if (sym.MidStyle.HasMid)
			style.AddChild(MidStyle(sym.MidStyle));

		node.AddChild(style);

		return node;
	}

	public static XMLNode TextSymbol(TextSymbol sym)
	{
		throw new NotImplementedException();
	}
	
	public static XMLNode DashStyle(DashStyle dash)
	{
		XMLNode node = new("DashStyle");
		
		node.AddAttribute("dashLength", dash.DashLength.ToString());
		node.AddAttribute("gapLength", dash.GapLength.ToString());
		node.AddAttribute("groupSize", dash.GroupSize.ToString());
		node.AddAttribute("groupGapLength", dash.GroupGapLength.ToString());

		return node;
	}

	public static XMLNode MidStyle(MidStyle mid)
	{
		XMLNode node = new("MidStyle");

		node.AddAttribute("gapLength", mid.GapLength.ToString());
		node.AddAttribute("requireMid", mid.RequireMid.ToString());
		node.AddAttribute("initialOffset", mid.InitialOffset.ToString());
		node.AddAttribute("endOffset", mid.EndOffset.ToString());

		node.AddChild(MapObjects(mid.MapObjects));

		return node;
	}

	#endregion
	
	#region Map Objects

	public static XMLNode MapObjects(IEnumerable<MapObject> objs)
	{
		XMLNode node = new("MapObjects");
		
		foreach (MapObject obj in objs)
			node.Children.Add(MapObject(obj));

		return node;
	}
	
	public static XMLNode MapObject(MapObject obj)
	{
		return obj switch
		{
			Phanes.Mapper.PointObject p => PointObject(p),
			Phanes.Mapper.LineObject l => LineObject(l),
			Phanes.Mapper.AreaObject a => AreaObject(a),
			Phanes.Mapper.TextObject t => TextObject(t),
			_ => throw new InvalidOperationException("Invalid map object type"),
		};
	}
	
	public static XMLNode PointObject(PointObject obj)
	{
		XMLNode node = new("PointObject");
		node.AddAttribute("id", obj.Id.ToString());
		
		XMLNode style = new("Style");
		XMLNode inner = new("Inner");
		XMLNode outer = new("Outer");

		XMLNode iColour = new("Colour")
		{
			InnerText = obj.InnerColour.Name == "Transparent"
				? "Transparent" : obj.InnerColour.Id.ToString(),
		};

		XMLNode iWidth = new("Width")
		{
			InnerText = obj.InnerRadius.ToString(),
		};

		inner.AddChild(iColour);
		inner.AddChild(iWidth);

		XMLNode oColour = new("Colour")
		{
			InnerText = obj.OuterColour.Name == "Transparent"
				? "Transparent" : obj.OuterColour.Id.ToString(),
		};

		XMLNode oWidth = new("Width")
		{
			InnerText = obj.OuterRadius.ToString(),
		};

		outer.AddChild(oColour);
		outer.AddChild(oWidth);

		style.AddChild(inner);
		style.AddChild(outer);

		node.AddChild(style);

		return node;
	}

	public static XMLNode LineObject(LineObject obj)
	{
		XMLNode node = new("LineObject");
		node.AddAttribute("id", obj.Id.ToString());
		
		XMLNode style = new("Style");

		XMLNode colour = new("Colour")
		{
			InnerText = obj.Colour.Name == "Transparent"
				? "Transparent" : obj.Colour.Id.ToString(),
		};

		XMLNode width = new("Width")
		{
			InnerText = obj.Width.ToString(),
		};

		style.AddChild(colour);
		style.AddChild(width);

		XMLNode points = new("Points");
		foreach (vec2 point in obj.Points)
			points.AddChild(Vec2(point));

		node.AddChild(style);
		node.AddChild(points);

		return node;
	}

	public static XMLNode AreaObject(AreaObject obj)
	{
		XMLNode node = new("LineObject");
		node.AddAttribute("id", obj.Id.ToString());

		XMLNode style = new("Style");

		XMLNode fill = Fill(obj.Fill);

		XMLNode border = new("Border");

		XMLNode borderCol = new("Colour")
		{
			InnerText = obj.BorderColour.Name == "Transparent"
				? "Transparent" : obj.BorderColour.Id.ToString(),
		};

		XMLNode borderWidth = new("Width")
		{
			InnerText = obj.Width.ToString(),
		};

		border.AddChild(borderCol);
		border.AddChild(borderWidth);

		style.AddChild(fill);
		style.AddChild(border);

		XMLNode points = new("Points");
		foreach (vec2 point in obj.Points)
			points.AddChild(Vec2(point));

		node.AddChild(style);
		node.AddChild(points);

		return node;
	}

	public static XMLNode TextObject(TextObject obj)
	{
		throw new NotImplementedException();
	}
	
	#endregion
	
	#region Instances

	public static XMLNode Instances(IEnumerable<Instance> insts)
	{
		XMLNode node = new("Instances");
		
		foreach (Instance inst in insts)
			node.Children.Add(Instance(inst));

		return node;
	}
	
	public static XMLNode Instance(Instance inst)
	{
		return inst switch
		{
			Phanes.Mapper.PointInstance p => PointInstance(p),
			Phanes.Mapper.LineInstance l => PathInstance(l),
			Phanes.Mapper.AreaInstance a => PathInstance(a),
			Phanes.Mapper.TextInstance t => TextInstance(t),
			_ => throw new InvalidOperationException("Invalid instance type"),
		};
	}
	
	public static XMLNode InstanceBase(Instance inst)
	{
		XMLNode node = new("Instance");
		
		node.AddAttribute("id", inst.Id.ToString());
		node.AddAttribute("layer", inst.Layer.ToString());

		XMLNode symbol = new("Symbol")
		{
			InnerText = inst.Symbol.Id.ToString(),
		};

		node.AddChild(symbol);

		return node;
	}

	public static XMLNode PointInstance(PointInstance inst)
	{
		XMLNode root = InstanceBase(inst);
		root.Name = "PointInstance";

		XMLNode centre = Vec2(inst.Centre);
		centre.Name = "Centre";

		root.AddChild(centre);

		return root;
	}

	public static XMLNode PathInstance(PathInstance inst)
	{
		XMLNode node = InstanceBase(inst);

		node.Name = inst is LineInstance 
			? "LineInstance" : "AreaInstance";

		node.AddChild(Segments(inst.Segments));

		return node;
	}

	public static XMLNode TextInstance(TextInstance inst)
	{
		throw new NotImplementedException();
	}
	
	#endregion
	
	#region Segments

	public static XMLNode Segments(PathCollection coll)
	{
		XMLNode node = new("Segments");

		foreach (IPathSegment seg in coll)
		{
			node.AddChild(seg switch
			{
				LinearPath line => LinearSegment(line),
				BezierPath bez => BezierSegment(bez),
				_ => throw new InvalidOperationException(),
			});
		}

		return node;
	}

	public static XMLNode LinearSegment(LinearPath line)
	{
		XMLNode	node = new("LinearPath");
		
		foreach (vec2 pt in line)
			node.AddChild(Vec2(pt));
		
		return node;
	}

	public static XMLNode BezierSegment(BezierPath bez)
	{
		XMLNode node = new("BezierPath");

		foreach (BezierPoint pt in bez)
		{
			XMLNode p = new("BezierPoint");

			XMLNode early = Vec2(pt.EarlyControl);
			early.Name = "EarlyControl";

			XMLNode anchor = Vec2(pt.Anchor);
			anchor.Name = "AnchorControl";
			
			XMLNode late = Vec2(pt.LateControl);
			late.Name = "LateControl";
			
			p.AddChild(early);
			p.AddChild(anchor);
			p.AddChild(late);
			
			node.AddChild(p);
		}
		
		return node;
	}
	
	#endregion

	private static XMLNode Vec2(vec2 v2)
	{
		XMLNode node = new("Point");

		node.AddAttribute("x", v2.X.ToString());
		node.AddAttribute("y", v2.Y.ToString());

		return node;
	}
}

#endregion