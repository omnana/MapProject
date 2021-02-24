using System.Collections.Generic;

public class FiguresModel : ITableModel
{
    public int Id { get; set; }
    public string Name  { get; set; }
    public string Introduce  { get; set; }
    public string FigurePicture  { get; set; }
    public string FigureLabel  { get; set; }
    public string FigureType  { get; set; }
    public string ExploreId  { get; set; }
    public string Explain  { get; set; }
    public string BackPicture  { get; set; }
    public string DetailImages  { get; set; }
    public string PosInUnity  { get; set; }
    public string IconForSetMeal  { get; set; }
    public string Brief  { get; set; }
    public string IntroOrigin  { get; set; }
    public string CircleIcon  { get; set; }
    public string ShareImage  { get; set; }
    public object Key()
    {
        return Id;
    }
}

public class FiguresModelMgr : TableManager<FiguresModel, FiguresModelMgr>
{
    public override string TableName()
    {
        return "Figures";
    }
    public override void InitModel(FiguresModel model, Dictionary<string, object> cellMap)
    {
        if (cellMap["Id"] != null)
            model.Id = int.Parse(cellMap["Id"].ToString());
        if (cellMap["Name"] != null)
            model.Name = cellMap["Name"].ToString();
        if (cellMap["Introduce"] != null)
            model.Introduce = cellMap["Introduce"].ToString();
        if (cellMap["FigurePicture"] != null)
            model.FigurePicture = cellMap["FigurePicture"].ToString();
        if (cellMap["FigureLabel"] != null)
            model.FigureLabel = cellMap["FigureLabel"].ToString();
        if (cellMap["FigureType"] != null)
            model.FigureType = cellMap["FigureType"].ToString();
        if (cellMap["ExploreId"] != null)
            model.ExploreId = cellMap["ExploreId"].ToString();
        if (cellMap["Explain"] != null)
            model.Explain = cellMap["Explain"].ToString();
        if (cellMap["BackPicture"] != null)
            model.BackPicture = cellMap["BackPicture"].ToString();
        if (cellMap["DetailImages"] != null)
            model.DetailImages = cellMap["DetailImages"].ToString();
        if (cellMap["PosInUnity"] != null)
            model.PosInUnity = cellMap["PosInUnity"].ToString();
        if (cellMap["IconForSetMeal"] != null)
            model.IconForSetMeal = cellMap["IconForSetMeal"].ToString();
        if (cellMap["Brief"] != null)
            model.Brief = cellMap["Brief"].ToString();
        if (cellMap["IntroOrigin"] != null)
            model.IntroOrigin = cellMap["IntroOrigin"].ToString();
        if (cellMap["CircleIcon"] != null)
            model.CircleIcon = cellMap["CircleIcon"].ToString();
        if (cellMap["ShareImage"] != null)
            model.ShareImage = cellMap["ShareImage"].ToString();
    }
}
