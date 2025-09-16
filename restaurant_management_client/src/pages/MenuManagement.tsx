/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useState } from "react";
import "./../styles/MenuManagement.css";
import { CategoryService } from "../services/CategoryService";
import { MenuItemService } from "../services/MenuItemService";
import type { CategoryResponse } from "../types/Category/CategoryResponse";
import type { MenuItemResponse } from "../types/MenuItem/MenuItemResponse";
import type { CreateMenuItemRequest } from "../types/MenuItem/CreateMenuItemRequest";
import type { CreateCategoryRequest } from "../types/Category/CreateCategoryRequest";
import { useAuth } from "../context/AuthProvider";
import { RestaurantService } from "../services/RestaurantService";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import {
  FaPlus,
  FaRegEdit,
  FaTrashAlt,
  FaTimes,
  FaSpinner,
  FaUtensils,
  FaFolderOpen,
  FaDollarSign,
  FaImage,
} from "react-icons/fa";
import type { UpdateMenuItemRequest } from "../types/MenuItem/UpdateMenuItemRequest";

export default function MenuManagement() {
  const { id } = useAuth();
  const [restaurantId, setRestaurantId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<{
    message: string;
    type: "success" | "error" | "info";
  } | null>(null);

  // kategori
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [newCategory, setNewCategory] = useState<{
    name: string;
    description: string;
  }>({
    name: "",
    description: "",
  });
  const [editingCategory, setEditingCategory] =
    useState<CategoryResponse | null>(null);

  // menu item
  const [newMenuItem, setNewMenuItem] = useState<
    Omit<CreateMenuItemRequest, "restaurantId" | "categoryId">
  >({
    name: "",
    description: "",
    price: 0,
    imageUrl: "",
  } as any);
  const [menuItems, setMenuItems] = useState<MenuItemResponse[]>([]);
  const [editingMenuItem, setEditingMenuItem] =
    useState<MenuItemResponse | null>(null);
  const [, setHubConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    fetchRestaurant();
  }, [id]);

  useEffect(() => {
    if (restaurantId) fetchCategories();
  }, [restaurantId]);

  useEffect(() => {
    if (selectedCategory && restaurantId) fetchMenuItems(selectedCategory);
    else setMenuItems([]);
  }, [selectedCategory, restaurantId]);

  const showStatusMessage = (
    message: string,
    type: "success" | "error" | "info"
  ) => {
    setStatusMessage({ message, type });
    setTimeout(() => setStatusMessage(null), 3000);
  };

  const fetchRestaurant = async () => {
    if (!id) return;
    setIsLoading(true);
    try {
      const res = await RestaurantService.getRestaurantByUserId();
      if (!res.success) {
        showStatusMessage(res.message || "Restoran bulunamadı.", "error");
        return;
      }
      setRestaurantId(res.data.id);
    } catch (err: any) {
      showStatusMessage("Restoran çağrılırken hata: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const fetchCategories = async () => {
    if (!restaurantId) return;
    setIsLoading(true);
    try {
      const res = await CategoryService.getCategoryByRestaurantId();
      if (!res.success) {
        showStatusMessage(res.message || "Kategori alınamadı.", "error");
        return;
      }
      setCategories(res.data || []);
    } catch (err: any) {
      showStatusMessage("Kategori çağrılırken hata: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const fetchMenuItems = async (categoryId: number) => {
    if (!restaurantId) return;
    setIsLoading(true);
    try {
      const res = await MenuItemService.getmenuItemsByRestaurantIdAndCategoryId(
        categoryId
      );
      if (!res.success) {
        showStatusMessage(res.message || "Menü öğeleri alınamadı.", "error");
        return;
      }
      setMenuItems(res.data || []);
    } catch (err: any) {
      showStatusMessage(
        "Menü öğeleri çağrılırken hata: " + err.message,
        "error"
      );
    } finally {
      setIsLoading(false);
    }
  };

  //Category CRUD
  const handleAddCategory = async () => {
    if (!restaurantId)
      return showStatusMessage("Restoran bilgisi yok.", "error");
    if (!newCategory.name.trim())
      return showStatusMessage("Kategori adı girin.", "error");

    setIsLoading(true);
    const payload: CreateCategoryRequest = {
      name: newCategory.name,
      description: newCategory.description,
    } as any;
    try {
      const res = await CategoryService.createCategory(payload);
      if (!res.success) {
        showStatusMessage(res.message || "Kategori oluşturulamadı.", "error");
        return;
      }
      setNewCategory({ name: "", description: "" });
      showStatusMessage("Kategori başarıyla eklendi.", "success");
      fetchCategories();
    } catch (err: any) {
      showStatusMessage("Kategori oluşturulamadı: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const handleEditCategorySubmit = async () => {
    if (!editingCategory || !restaurantId) return;
    setIsLoading(true);
    try {
      const payload = {
        id: editingCategory.id,
        name: editingCategory.name,
        description: editingCategory.description,
      };
      const res = await CategoryService.updateCategory(payload as any);
      if (!res.success) {
        showStatusMessage(res.message || "Güncelleme başarısız.", "error");
        return;
      }
      setEditingCategory(null);
      showStatusMessage("Kategori başarıyla güncellendi.", "success");
      fetchCategories();
    } catch (err: any) {
      showStatusMessage("Kategori güncellenemedi: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteCategory = async (categoryId: number) => {
    if (!window.confirm("Bu kategoriyi silmek istediğinize emin misiniz?"))
      return;
    setIsLoading(true);
    try {
      const res = await CategoryService.deleteCategory(categoryId);
      if (!res.success) {
        showStatusMessage(res.message || "Kategori silinemedi.", "error");
        return;
      }
      if (selectedCategory === categoryId) {
        setSelectedCategory(null);
        setMenuItems([]);
      }
      showStatusMessage("Kategori başarıyla silindi.", "success");
      fetchCategories();
    } catch (err: any) {
      showStatusMessage("Kategori silinirken hata: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  // MenuItem CRUD
  const handleAddMenuItem = async () => {
    if (!restaurantId || !selectedCategory)
      return showStatusMessage("Kategori seçin.", "error");
    if (!newMenuItem.name.trim())
      return showStatusMessage("Ürün adı girin.", "error");
    setIsLoading(true);
    const payload: CreateMenuItemRequest = {
      categoryId: selectedCategory,
      name: newMenuItem.name,
      description: newMenuItem.description,
      price: newMenuItem.price,
      imageUrl: newMenuItem.imageUrl,
    };
    try {
      const res = await MenuItemService.createMenuItem(payload);
      if (!res.success) {
        showStatusMessage(res.message || "Menü öğesi oluşturulamadı.", "error");
        return;
      }
      setNewMenuItem({
        name: "",
        description: "",
        price: 0,
        imageUrl: "",
      } as any);
      showStatusMessage("Menü öğesi başarıyla eklendi.", "success");
      fetchMenuItems(selectedCategory);
    } catch (err: any) {
      showStatusMessage("Menü öğesi oluşturulamadı: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const openEditMenuItem = (item: MenuItemResponse) => {
    setEditingMenuItem(item);
  };

  const handleEditMenuItemSubmit = async () => {
    if (!editingMenuItem || !restaurantId) return;
    setIsLoading(true);
    try {
      const payload: UpdateMenuItemRequest = {
        id: editingMenuItem.id,
        categoryId: editingMenuItem.categoryId,
        name: editingMenuItem.name,
        description: editingMenuItem.description,
        price: editingMenuItem.price,
        imageUrl: editingMenuItem.imageUrl,
      };
      const res = await MenuItemService.updateMenuItem(payload);
      if (!res.success) {
        showStatusMessage(res.message || "Güncelleme başarısız.", "error");
        return;
      }
      setEditingMenuItem(null);
      showStatusMessage("Menü öğesi başarıyla güncellendi.", "success");
      if (selectedCategory) fetchMenuItems(selectedCategory);
    } catch (err: any) {
      showStatusMessage("Menü öğesi güncellenemedi: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteMenuItem = async (menuItemId: number) => {
    if (!window.confirm("Bu ürünü silmek istediğinize emin misiniz?")) return;
    setIsLoading(true);
    try {
      const res = await MenuItemService.deleteMenuItem(menuItemId);
      if (!res.success) {
        showStatusMessage(res.message || "Silinemedi.", "error");
        return;
      }
      showStatusMessage("Menü öğesi başarıyla silindi.", "success");
      if (selectedCategory) fetchMenuItems(selectedCategory);
    } catch (err: any) {
      showStatusMessage("Menü öğesi silinirken hata: " + err.message, "error");
    } finally {
      setIsLoading(false);
    }
  };

  // helper
  const placeholderImg = (url?: string) =>
    url && url.length > 0
      ? url
      : "https://via.placeholder.com/300x200?text=No+Image";

  //hub
  useEffect(() => {
    if (!restaurantId) return;

    // Hub bağlantısı
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5164/hubs", { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        console.log("SignalR connected");
        connection.invoke("JoinRestaurantGroup", restaurantId);

        connection.on("MenuItemChanged", () => {
          if (selectedCategory) {
            fetchMenuItems(selectedCategory);
          }
        });
        connection.on("CategoryChanged", () => {
          fetchCategories();
        });
      })
      .catch((err) => console.error("SignalR connection error:", err));
    setHubConnection(connection);
    return () => {
      connection.stop();
    };
  }, [restaurantId]);

  return (
    <div className="menu-management-container">
      <div className="menu-management-header">
        <h2 className="page-title">Menü Yönetimi</h2>
        {isLoading && (
          <p className="loading-message">
            <FaSpinner className="spinner" /> İşlem yapılıyor...
          </p>
        )}
        {statusMessage && (
          <div className={`status-message ${statusMessage.type}`}>
            {statusMessage.message}
          </div>
        )}
      </div>

      <div className="main-content">
        {/* Kategori Paneli */}
        <div className="content-card category-panel">
          <div className="content-header">
            <h3>
              <FaFolderOpen /> Kategoriler
            </h3>
          </div>
          <div className="category-form">
            <input
              placeholder="Kategori adı"
              value={newCategory.name}
              onChange={(e) =>
                setNewCategory({ ...newCategory, name: e.target.value })
              }
            />
            <input
              placeholder="Açıklama"
              value={newCategory.description}
              onChange={(e) =>
                setNewCategory({ ...newCategory, description: e.target.value })
              }
            />
            <button className="primary-btn" onClick={handleAddCategory}>
              <FaPlus /> Kategori Ekle
            </button>
          </div>
          <div className="category-list">
            {categories.length > 0 ? (
              categories.map((cat) => (
                <div
                  key={cat.id}
                  className={`category-item ${
                    selectedCategory === cat.id ? "active" : ""
                  }`}
                >
                  <button
                    className="category-button"
                    onClick={() => setSelectedCategory(cat.id)}
                  >
                    {cat.name}
                  </button>
                  <div className="category-actions">
                    <button
                      title="Düzenle"
                      className="icon-btn edit-btn"
                      onClick={() => setEditingCategory(cat)}
                    >
                      <FaRegEdit />
                    </button>
                    <button
                      title="Sil"
                      className="icon-btn delete-btn"
                      onClick={() => handleDeleteCategory(cat.id)}
                    >
                      <FaTrashAlt />
                    </button>
                  </div>
                </div>
              ))
            ) : (
              <p className="muted-text">Henüz kategori eklenmedi.</p>
            )}
          </div>
        </div>

        {/* Menü Öğeleri Paneli */}
        <div className="content-card menu-items-panel">
          <div className="content-header">
            <h3>
              <FaUtensils /> Menü Öğeleri
            </h3>
            {selectedCategory && (
              <span className="selected-category-badge">
                {categories.find((c) => c.id === selectedCategory)?.name}
              </span>
            )}
          </div>
          {selectedCategory ? (
            <>
              <div className="menu-item-form">
                <div className="form-group-icon">
                  <FaUtensils />
                  <input
                    placeholder="Ürün adı"
                    value={newMenuItem.name}
                    onChange={(e) =>
                      setNewMenuItem({ ...newMenuItem, name: e.target.value })
                    }
                  />
                </div>
                <div className="form-group-icon">
                  <FaRegEdit />
                  <textarea
                    placeholder="Açıklama"
                    value={newMenuItem.description}
                    onChange={(e) =>
                      setNewMenuItem({
                        ...newMenuItem,
                        description: e.target.value,
                      })
                    }
                  />
                </div>
                <div className="form-row">
                  <div className="form-group-icon">
                    <FaDollarSign />
                    <input
                      type="number"
                      placeholder="Fiyat"
                      value={newMenuItem.price}
                      onChange={(e) =>
                        setNewMenuItem({
                          ...newMenuItem,
                          price: Number(e.target.value),
                        })
                      }
                    />
                  </div>
                  <br />
                  <div className="form-group-icon">
                    <FaImage />
                    <input
                      placeholder="Resim URL"
                      value={newMenuItem.imageUrl}
                      onChange={(e) =>
                        setNewMenuItem({
                          ...newMenuItem,
                          imageUrl: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
                <button className="primary-btn" onClick={handleAddMenuItem}>
                  <FaPlus /> Ürün Ekle
                </button>
              </div>

              <div className="menu-grid">
                {menuItems.length > 0 ? (
                  menuItems.map((item) => (
                    <div key={item.id} className="menu-card">
                      <img
                        src={placeholderImg(item.imageUrl)}
                        alt={String(item.name)}
                      />
                      <div className="menu-card-body">
                        <h4>{String(item.name)}</h4>
                        <p className="desc">{item.description}</p>
                        <div className="price">{item.price} ₺</div>
                        <div className="actions">
                          <button
                            className="secondary-btn"
                            onClick={() => openEditMenuItem(item)}
                          >
                            <FaRegEdit /> Düzenle
                          </button>
                          <button
                            className="danger-btn"
                            onClick={() => handleDeleteMenuItem(item.id)}
                          >
                            <FaTrashAlt /> Sil
                          </button>
                        </div>
                      </div>
                    </div>
                  ))
                ) : (
                  <p className="muted-text">Bu kategoride henüz ürün yok.</p>
                )}
              </div>
            </>
          ) : (
            <p className="info-message">
              Menü öğelerini görmek için bir kategori seçin.
            </p>
          )}
        </div>
      </div>

      {/* Kategori Düzenleme Modalı */}
      {editingCategory && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h3>Kategori Düzenle</h3>
              <button
                className="modal-close-btn"
                onClick={() => setEditingCategory(null)}
              >
                <FaTimes />
              </button>
            </div>
            <div className="modal-body">
              <div className="modal-form-container">
                <input
                  value={editingCategory.name}
                  onChange={(e) =>
                    setEditingCategory({
                      ...editingCategory,
                      name: e.target.value,
                    })
                  }
                  placeholder="Kategori Adı"
                />
                <textarea
                  value={editingCategory.description}
                  onChange={(e) =>
                    setEditingCategory({
                      ...editingCategory,
                      description: e.target.value,
                    })
                  }
                  placeholder="Açıklama"
                />
              </div>
            </div>
            <div className="modal-actions">
              <button
                className="primary-btn"
                onClick={handleEditCategorySubmit}
              >
                Kaydet
              </button>
              <button
                className="secondary-btn"
                onClick={() => setEditingCategory(null)}
              >
                İptal
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Menü Öğesi Düzenleme Modalı */}
      {editingMenuItem && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h3>Menü Öğesi Düzenle</h3>
              <button
                className="modal-close-btn"
                onClick={() => setEditingMenuItem(null)}
              >
                <FaTimes />
              </button>
            </div>
            <div className="modal-body">
              <div className="modal-form-container">
                <div className="form-group-icon">
                  <FaUtensils />
                  <input
                    value={String(editingMenuItem.name)}
                    onChange={(e) =>
                      setEditingMenuItem({
                        ...editingMenuItem,
                        name: e.target.value,
                      })
                    }
                    placeholder="Ürün Adı"
                  />
                </div>
                <div className="form-group-icon">
                  <FaRegEdit />
                  <textarea
                    value={editingMenuItem.description}
                    onChange={(e) =>
                      setEditingMenuItem({
                        ...editingMenuItem,
                        description: e.target.value,
                      })
                    }
                    placeholder="Açıklama"
                  />
                </div>
                <div className="form-group-icon">
                  <FaDollarSign />
                  <input
                    type="number"
                    value={editingMenuItem.price}
                    onChange={(e) =>
                      setEditingMenuItem({
                        ...editingMenuItem,
                        price: Number(e.target.value),
                      })
                    }
                    placeholder="Fiyat"
                  />
                </div>
                <div className="form-group-icon">
                  <FaImage />
                  <input
                    value={editingMenuItem.imageUrl}
                    onChange={(e) =>
                      setEditingMenuItem({
                        ...editingMenuItem,
                        imageUrl: e.target.value,
                      })
                    }
                    placeholder="Resim URL"
                  />
                </div>
              </div>
            </div>
            <div className="modal-actions">
              <button
                className="primary-btn"
                onClick={handleEditMenuItemSubmit}
              >
                Kaydet
              </button>
              <button
                className="secondary-btn"
                onClick={() => setEditingMenuItem(null)}
              >
                İptal
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
