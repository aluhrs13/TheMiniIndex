import authService from "../utils/AuthorizeService.js";
import { perfMark, perfMeasure } from "../utils/PerformanceMarks";

export async function getMinis(): Promise<Mini[]> {
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

export async function getMiniDetail(
  id: number | null
): Promise<DetailedMini | null> {
  if (id === null) return null;

  perfMark("tmi-getMiniDetail-start");
  const token = await authService.getAccessToken();
  const response = await fetch("https://localhost:44386/api/minis/" + id, {
    headers: !token ? {} : { Authorization: `Bearer ${token}` },
  });

  perfMark("tmi-getMiniDetail-end");
  perfMeasure(
    "tmi-getMiniDetail",
    "tmi-getMiniDetail-start",
    "tmi-getMiniDetail-end"
  );
  if (response.ok) {
    return await response.json();
  } else {
    return null;
  }
}

export type Mini = {
  ID: number;
  Name: string;
  Status: number;
  Creator: {
    id: number;
    name: string;
  };
  Thumbnail: string;
  LinuxTime: number;
};

export type DetailedMini = {
  ID: number;
  Name: string;
  Link: string;
  Thumbnail: string;
  Creator: {
    ID: number;
    Name: string;
  };
  Status: number;
  MiniTags: MiniTag[];
  Sources: string[];
  ApprovedTime: string;
};

export type MiniTag = {
  MiniID: number;
  TagID: number;
  Tag: { ID: number; TagName: string; Category: string };
  Status: number;
  CreatedTime: string;
  ApprovedTime: string;
  LastModifiedTime: string;
};

export enum TagCategory {
  Gender = 0,
  Race,
  Genre,
  Use,
  Size,
  Alignment,
  CreatureType,
  CreatureName,
  Class,
  Weapon,
  Armor,
  Clothing,
  Location,
  OtherDescription,
  Purpose,
  Scale,
  SourceBook,
  BookSection,
}

export type Tag = {
  Status: string;
  TagName: string;
  ID: number;
  Category: string;
};
