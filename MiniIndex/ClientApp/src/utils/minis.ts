import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";

export async function getMinis(initLoad?: boolean): Promise<Mini[]> {
  perfMark("tmi-getMiniList-start");
  const token = await authService.getAccessToken();
  const response = await fetch("https://localhost:44386/api/minis", {
    headers: !token ? {} : { Authorization: `Bearer ${token}` },
  });
  const data = await response.json();
  perfMark("tmi-getMiniList-end");
  perfMeasure(
    "tmi-getMiniList",
    "tmi-getMiniList-start",
    "tmi-getMiniList-end"
  );
  return data;
}

export async function getMiniDetail(id: number): Promise<DetailedMini> {
  perfMark("tmi-getMiniDetail-start");
  const token = await authService.getAccessToken();
  const response = await fetch("https://localhost:44386/api/minis/" + id, {
    headers: !token ? {} : { Authorization: `Bearer ${token}` },
  });
  const data = await response.json();
  perfMark("tmi-getMiniDetail-end");
  perfMeasure(
    "tmi-getMiniDetail",
    "tmi-getMiniDetail-start",
    "tmi-getMiniDetail-end"
  );
  return data;
}

export type Mini = {
  id: number;
  name: string;
  status: number;
  creator: {
    id: number;
    name: string;
  };
  thumbnail: string;
  linuxTime: number;
};

export type DetailedMini = {
  ID: number;
  Name: string;
  Link: string;
  Thumbnail: string;
  creator: {
    ID: number;
    Name: string;
  };
  Status: number;
  MiniTags: {
    $values: [
      {
        MiniID: number;
        TagID: number;
        Tag: { ID: number; Name: string };
        CreatedTime: string;
        ApprovedTime: string;
        LastModifiedTime: string;
      }
    ];
  };
  Sources: {
    $values: [];
  };
  ApprovedTime: string;
};
