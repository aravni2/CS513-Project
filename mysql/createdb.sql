create database nypl;
use nypl;

CREATE TABLE Dishes(
      id integer NOT NULL,
      name VARCHAR(1387),
      description VARCHAR(255), -- 0 length
      menus_appeared integer,
      times_appeared integer,
      first_appeared integer,
      last_appeared integer,
      lowest_price real,
      highest_price real,
	  PRIMARY KEY(id)
	  );

CREATE TABLE Menus(
      id integer NOT NULL,
      name VARCHAR(77),
      sponsor VARCHAR(127),
      event VARCHAR(194),
      venue VARCHAR(47),
      place VARCHAR(106),
      physical_description VARCHAR(118),
      occasion VARCHAR(97),
      notes VARCHAR(255), -- 0 length
      call_number VARCHAR(40),
      keywords VARCHAR(255), -- 0 length
      language VARCHAR(255), -- 0 length
      date DATE,
      location VARCHAR(127),
      location_type VARCHAR(255), -- 0 length
      currency VARCHAR(26),
      currency_symbol VARCHAR(4),
      status VARCHAR(12),
      page_count integer,
      dish_count integer,
	  PRIMARY KEY (id)
	  );

CREATE TABLE MenuItems(
      id integer NOT NULL,
      menu_page_id integer,
      price real,
      high_price real,
      dish_id integer,
      created_at DATE,
      updated_at DATE,
      xpos real,
      ypos real,
	  PRIMARY KEY (id),
	  FOREIGN KEY (dish_id) REFERENCES Dishes(id)
	  );

CREATE TABLE MenuPages(
      id integer NOT NULL,
      menu_id integer,
      page_number integer,
      image_id integer,
      full_height integer,
      full_width integer,
      uuid VARCHAR(36),
	  PRIMARY KEY (id),
	  FOREIGN KEY (menu_id) REFERENCES MenuItems(id)
	  );

ALTER TABLE MenuItems
ADD CONSTRAINT FK_MenuPage_id
FOREIGN KEY (menu_page_id) REFERENCES MenuPages(id);
