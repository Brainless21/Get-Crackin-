using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    public bool forcedSquares;
    public int rows; // wie viele reihen es am ende gibt
    public int columns; // wie viele zeilen es am ende gibt
    public Vector2 cellSize; // wie groß jede zelle sein kann
    public Vector2 spacing;
    public bool fitX;
    public bool fitY;
    public FitType fitType;
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        float sqrRt = Mathf.Sqrt(transform.childCount); // wurzel der anzuordnenden objekte
       
       if(fitType == FitType.Width ||fitType == FitType.Height || fitType == FitType.Uniform)
       {
            // die wurzel aufgerunded ergibt die anzahl an reihen und spalten die benötigt werden um alles unterzubringen (squarely)
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
            fitX = true;
            fitY = true;
       }

        if(fitType==FitType.FixedRows)
        {
            // da fehlt doch bestimmt noch was
        }

        if(fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }

        if(fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        // hier wird geschaut, wie viel platz insgesamt zur verfügung steht
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // errechnet, basierend auf dem padding, spacing und #of rows/columns wie viel space den objekten am ende tatsächlich zur verfügung steht
        float availableWidth = parentWidth - padding.right - padding.left - (spacing.x * (columns-1));
        float availableHeight = parentHeight - padding.top - padding.bottom - (spacing.y * (rows-1));

        // daraus wird dann hier errechnet, wie groß eine zelle sein kann, um ins grid zu passen. der verfügbare platz wird aufgeteilt auf die nummer an reihen und zeilen
        float cellWidth = availableWidth / (float)columns;
        float cellHeight = availableHeight / (float)rows;

        // wenn die elemente squared sein müssen, wird die größe der längeren seite auf die der kürzeren reduziert
        if(forcedSquares==true)
        {
            if(cellHeight<cellWidth) cellWidth = cellHeight;
            if(cellHeight>cellWidth) cellHeight = cellWidth;
        }

        // set the cellSize Vector with the newfound information
        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight: cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for(int i = 0; i < rectChildren.Count; i++)
        {
            // errechnet die "koordinaten" unserer objekte im Grid, also reihen und zeilennummer
            rowCount = i / columns; // weird typecasting but I guess it always rounds down, so this works when were starting to count both index and rows from 0)
            columnCount = i % columns; // again, since were counting from 0, this actually magically works

            var item = rectChildren[i]; //reference to child object

            // bestimmt die position des objectes im rect
            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.right;

            // plaziert das objekt an seinem passenden platz
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        //throw new System.NotImplementedException();
    }
}
